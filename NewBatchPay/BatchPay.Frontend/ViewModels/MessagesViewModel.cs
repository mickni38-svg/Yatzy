using System.Collections.ObjectModel;
using BatchPay.Contracts.Dto;
using BatchPay.Frontend.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatchPay.Frontend.ViewModels;

public partial class MessagesViewModel : ObservableObject
{
    private readonly BatchPayApiClient _api;
    private readonly IUserContext _userContext;

    private int CurrentUserId => _userContext.CurrentUserId ?? 0;

    public ObservableCollection<NotificationDto> Notifications { get; } = new();

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? statusMessage;

    public MessagesViewModel( BatchPayApiClient api, IUserContext userContext )
    {
        _userContext = userContext;
        _api = api;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            StatusMessage = null;
            Notifications.Clear();

            if (CurrentUserId <= 0)
            {
                StatusMessage = "CurrentUserId er 0/null. Notifications kan ikke hentes før user context er sat.";
                return;
            }

            var list = await _api.GetNotificationsForUserAsync( CurrentUserId, CancellationToken.None );

            foreach (var n in list)
                Notifications.Add( n );

            StatusMessage = $"Loaded {Notifications.Count} beskeder for userId={CurrentUserId}.";
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
    public async Task OpenAsync( NotificationDto n )
    {
        var url = n?.LinkUrl?.Trim();
        if (string.IsNullOrWhiteSpace( url ))
        {
            StatusMessage = "Beskeden har intet link.";
            return;
        }

        if (!Uri.TryCreate( url, UriKind.Absolute, out var uri ))
        {
            StatusMessage = "LinkUrl er ikke et gyldigt URL-format.";
            return;
        }

        await Launcher.Default.OpenAsync( uri );
    }
}