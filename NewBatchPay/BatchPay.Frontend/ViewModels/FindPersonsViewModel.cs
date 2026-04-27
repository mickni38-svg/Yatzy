using BatchPay.Contracts.Dto;
using BatchPay.Frontend.Models;
using BatchPay.Frontend.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BatchPay.Frontend.ViewModels;

public partial class FindPersonsViewModel : ObservableObject
{
    private readonly BatchPayApiClient _api;

    private readonly IUserContext _userContext;

    private int CurrentUserId => _userContext.CurrentUserId ?? 0;

    // ✅ UI-liste (Users + Merchants) som kan markeres (kun users)
    public ObservableCollection<SelectableDirectoryEntry> AllEntries { get; } = new();
    public ObservableCollection<SelectableDirectoryEntry> FilteredEntries { get; } = new();

    [ObservableProperty]
    private string searchText = "";

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? statusMessage;

    public bool HasSelection => AllEntries.Any( x => x.IsSelected );

    public FindPersonsViewModel( BatchPayApiClient api, IUserContext userContext )
    {
        _api = api;
        _userContext = userContext;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        StatusMessage = null;

        try
        {
            AllEntries.Clear();
            FilteredEntries.Clear();

            // 1) hent eksisterende venner (kun users)
            var friends = await _api.GetFriendsAsync( CurrentUserId, CancellationToken.None );
            var friendIds = friends.Select( f => f.Id ).ToHashSet();

            // 2) hent directory (users + merchants)
            var entries = await _api.GetDirectoryAsync( CancellationToken.None );

            foreach (var dto in entries)
            {
                // skjul mig selv
                if (dto.Type == DirectoryEntryType.User && dto.Id == CurrentUserId)
                    continue;

                // skjul allerede-venner (users)
                if (friendIds.Contains( dto.Id ))
                    continue;

                AllEntries.Add( new SelectableDirectoryEntry( dto ) );
            }

            ApplyFilter();
            OnPropertyChanged( nameof( HasSelection ) );
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSearchTextChanged( string value )
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        FilteredEntries.Clear();

        var q = (SearchText ?? "").Trim();

        var list = string.IsNullOrWhiteSpace( q )
            ? AllEntries
            : AllEntries.Where( x =>
                x.DisplayName.Contains( q, StringComparison.OrdinalIgnoreCase ) ||
                x.Handle.Contains( q, StringComparison.OrdinalIgnoreCase ) );

        foreach (var e in list)
            FilteredEntries.Add( e );
    }

    // ✅ Bruges af XAML TapGesture på ringen/checkbox
    [RelayCommand]
    public void ToggleSelect( SelectableDirectoryEntry? entry )
    {
        if (entry is null) return;
        if (!entry.IsSelectable) return; // merchants kan ikke vælges i MVP

        entry.IsSelected = !entry.IsSelected;
        OnPropertyChanged( nameof( HasSelection ) );
    }

    [RelayCommand]
    public async Task AddSelectedAsync()
    {
        if (IsBusy) return;

        var selectedUsers = AllEntries
            .Where( x =>  x.IsSelected )
            .ToList();

        if (selectedUsers.Count == 0)
            return;

        IsBusy = true;
        StatusMessage = null;

        try
        {
            foreach (var u in selectedUsers)
            {
                await _api.AddFriendAsync( CurrentUserId, u.Id, CancellationToken.None );
                u.IsSelected = false;
            }

            StatusMessage = "Tilføjet til vennelisten.";

            foreach (var u in selectedUsers)
                AllEntries.Remove( u );

            ApplyFilter();
            OnPropertyChanged( nameof( HasSelection ) );
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public void OpenFindPersons()
    {
        ApplyFilter();
    }
}
