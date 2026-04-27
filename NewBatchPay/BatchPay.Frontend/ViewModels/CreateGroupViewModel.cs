using BatchPay.Contracts.Dto;
using BatchPay.Contracts.Icons;
using BatchPay.Frontend.Models;
using BatchPay.Frontend.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BatchPay.Frontend.ViewModels;

public partial class CreateGroupViewModel : ObservableObject
{
    private readonly BatchPayApiClient _api;
    private readonly IUserContext _userContext;

    private int CurrentUserId => _userContext.CurrentUserId ?? 0;

    public ObservableCollection<SelectableDirectoryEntry> PotentialMembers { get; } = new();
    public ObservableCollection<SelectableDirectoryEntry> FilteredPotentialMembers { get; } = new();
    public ObservableCollection<SelectableIconOption> IconOptions { get; } = new();

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string message = string.Empty;

    [ObservableProperty]
    private string iconKey = "icon_users";

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? statusMessage;

    [ObservableProperty]
    private string friendSearchText = string.Empty;

    [ObservableProperty]
    private SelectableIconOption? selectedIcon;

    public bool HasSelection => PotentialMembers.Any( x => x.IsSelected );

    public CreateGroupViewModel( BatchPayApiClient api, IUserContext userContext )
    {
        _api = api;
        _userContext = userContext;
    }

    partial void OnFriendSearchTextChanged( string value ) => ApplyFriendFilter();

    private void ApplyFriendFilter()
    {
        FilteredPotentialMembers.Clear();

        var query = FriendSearchText.Trim();
        var items = PotentialMembers.AsEnumerable();

        if (!string.IsNullOrWhiteSpace( query ))
        {
            items = items.Where( x =>
                (x.DisplayName?.Contains( query, StringComparison.OrdinalIgnoreCase ) ?? false) ||
                (x.Handle?.Contains( query, StringComparison.OrdinalIgnoreCase ) ?? false) );
        }

        foreach (var item in items)
            FilteredPotentialMembers.Add( item );
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;

        if (CurrentUserId <= 0)
        {
            StatusMessage = "Du skal logge ind først (vælg bruger på Home).";
            return;
        }

        IsBusy = true;
        StatusMessage = null;
        PotentialMembers.Clear();
        FilteredPotentialMembers.Clear();
        IconOptions.Clear();

        try
        {
            var friends = await _api.GetFriendsAsync( CurrentUserId, CancellationToken.None );
            var usersAndMerchants = await _api.GetDirectoryAsync( CancellationToken.None );

            var merchants = usersAndMerchants.Where( x => x.Type == DirectoryEntryType.Merchant );

            foreach (var friend in friends)
            {
                if (friend.Id == CurrentUserId)
                    continue;
                var type = merchants.Where( m => m.Id == friend.Id ).FirstOrDefault()?.Type ?? DirectoryEntryType.User;

                var dto = new DirectoryEntryDto(
                    type,
                    friend.Id,
                    friend.DisplayName,
                    friend.Handle );

                PotentialMembers.Add( new SelectableDirectoryEntry( dto ) { IsSelected = false } );
            }

            foreach (var key in IconCatalog.Keys)
                IconOptions.Add( new SelectableIconOption( $"icon_{key}" ) );

            ApplyFriendFilter();
            OnPropertyChanged( nameof( HasSelection ) );
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading members: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public void ToggleSelect( SelectableDirectoryEntry? entry )
    {
        if (entry is null || !entry.IsSelectable) return;

        entry.IsSelected = !entry.IsSelected;
        OnPropertyChanged( nameof( HasSelection ) );
    }

    [RelayCommand]
    public void ToggleFriend( SelectableDirectoryEntry? entry ) => ToggleSelect( entry );

    [RelayCommand]
    public void OpenFriendSearch() => ApplyFriendFilter();

    [RelayCommand]
    public void SelectIcon( SelectableIconOption? icon )
    {
        if (icon is null) return;

        // Force change hvis samme item vælges igen
        if (ReferenceEquals( SelectedIcon, icon ))
            SelectedIcon = null;

        SelectedIcon = icon;
    }

    partial void OnSelectedIconChanged( SelectableIconOption? value )
    {
        if (value is null) return;

        foreach (var option in IconOptions)
            option.IsSelected = ReferenceEquals( option, value );

        IconKey = value.IconKey;
    }

    [RelayCommand]
    public async Task CreateGroup()
    {
        if (IsBusy) return;

        if (CurrentUserId <= 0)
        {
            StatusMessage = "Du skal logge ind først (vælg bruger på Home).";
            return;
        }

        if (!HasSelection || string.IsNullOrWhiteSpace( Title ))
        {
            StatusMessage = "Title and at least one member are required.";
            return;
        }

        IsBusy = true;
        StatusMessage = null;

        try
        {
            var selectedMemberIds = PotentialMembers
                .Where( m => m.IsSelected )
                .Select( m => m.Id )
                .ToList();

            var merchantid = PotentialMembers.Where( m => m.IsSelected && m.IsMerchant)
                .Select( m => m.Id ).FirstOrDefault();

            var request = new CreateGroupPaymentRequestDto(
                CurrentUserId,
                merchantid,
                title,
                Message,
                IconKey,
                IsActive: true,
                CreatedAtUtc: DateTime.UtcNow,
                selectedMemberIds );

            await _api.CreateGroupPaymentAsync( request, CancellationToken.None );

            StatusMessage = "Group created successfully!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error creating group: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}