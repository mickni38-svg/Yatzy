using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BatchPay.Contracts.Dto;
using BatchPay.Frontend.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics;

namespace BatchPay.Frontend.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly BatchPayApiClient _api;
    private readonly IUserContext _userContext;

    public ObservableCollection<UserDto> Users { get; } = new();
    public ObservableCollection<UserDto> Friends { get; } = new();
    public ObservableCollection<HomeParticipantChipVm> ActiveParticipants { get; } = new();

    [ObservableProperty]
    private UserDto? selectedUser;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? statusMessage;

    [ObservableProperty]
    private string activeOrderTitle = "Ingen aktiv ordre";

    [ObservableProperty]
    private string activeMerchantName = "";

    [ObservableProperty]
    private string activeOrderProgressText = "";

    [ObservableProperty]
    private string activeOrderInfoText = "";

    [ObservableProperty]
    private string activeOrderActionText = "Åbn";

    [ObservableProperty]
    private string activeOrderIconSource = "icon_users";

    [ObservableProperty]
    private bool hasActiveGroupPayment;

    [ObservableProperty]
    private int paidMembersCount;

    [ObservableProperty]
    private int totalMembersCount;

    [ObservableProperty]
    private string notificationTitle = "Ingen notifikationer";

    [ObservableProperty]
    private string notificationSubtitle = "Log ind for at se dine notifikationer";

    [ObservableProperty]
    private bool isNotificationUnread;

    public bool IsLoggedIn => _userContext.CurrentUserId is not null;

    public string LoggedInAsText
        => SelectedUser is null ? "Ikke logget ind" : $"Logget ind som: {SelectedUser.DisplayName}";

    public string GreetingName
        => string.IsNullOrWhiteSpace( SelectedUser?.DisplayName ) ? "der" : SelectedUser.DisplayName;

    public Color NotificationCardBackground
        => IsNotificationUnread ? Color.FromArgb( "#0B1A2A" ) : Color.FromArgb( "#09131E" );

    public Color NotificationCardStroke
        => IsNotificationUnread ? Color.FromArgb( "#1E90C2" ) : Color.FromArgb( "#173247" );

    public double NotificationCardStrokeThickness
        => IsNotificationUnread ? 1.6 : 1.0;

    public string NotificationSubtitleColor
        => IsNotificationUnread ? "#C7D8E8" : "#8FA4B8";

    public FontAttributes NotificationTitleAttributes
        => IsNotificationUnread ? FontAttributes.Bold : FontAttributes.None;

    public HomeViewModel( BatchPayApiClient api, IUserContext userContext )
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
            Users.Clear();

            var users = await _api.GetAllUsersAsync( CancellationToken.None );
            foreach (var u in users.OrderBy( x => x.DisplayName ))
                Users.Add( u );

            OnPropertyChanged( nameof( GreetingName ) );

            var currentId = _userContext.CurrentUserId;
            if (currentId is not null)
            {
                SelectedUser = Users.FirstOrDefault( x => x.Id == currentId.Value );
                OnPropertyChanged( nameof( IsLoggedIn ) );

                if (SelectedUser is not null)
                {
                    Friends.Clear();
                    var friends = await _api.GetFriendsAsync( SelectedUser.Id, CancellationToken.None );
                    foreach (var f in friends)
                        Friends.Add( f );
                }

                await LoadActiveGroupPaymentSummaryAsync( currentId.Value );
            }
            else
            {
                ResetActiveGroupPaymentSummary();
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading users: {ex.Message}";
            ResetActiveGroupPaymentSummary();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task LoginAsync()
    {
        if (IsBusy) return;

        if (SelectedUser is null)
        {
            StatusMessage = "Vælg en bruger først.";
            return;
        }

        IsBusy = true;
        StatusMessage = null;

        try
        {
            _userContext.CurrentUserId = SelectedUser.Id;
            OnPropertyChanged( nameof( IsLoggedIn ) );

            Friends.Clear();
            var friends = await _api.GetFriendsAsync( SelectedUser.Id, CancellationToken.None );
            foreach (var f in friends)
                Friends.Add( f );

            await LoadActiveGroupPaymentSummaryAsync( SelectedUser.Id );

            StatusMessage = $"Logget ind som: {SelectedUser.DisplayName}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Login failed: {ex.Message}";
            ResetActiveGroupPaymentSummary();
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedUserChanged( UserDto? value )
    {
        OnPropertyChanged( nameof( LoggedInAsText ) );
        OnPropertyChanged( nameof( GreetingName ) );
    }

    [RelayCommand]
    public void Logout()
    {
        _userContext.CurrentUserId = null;
        SelectedUser = null;
        Friends.Clear();
        StatusMessage = "Logget ud.";
        ResetActiveGroupPaymentSummary();
        OnPropertyChanged( nameof( IsLoggedIn ) );
        OnPropertyChanged( nameof( LoggedInAsText ) );
        OnPropertyChanged( nameof( GreetingName ) );
    }

    public void MarkNotificationAsRead()
    {
        if (!IsNotificationUnread)
            return;

        IsNotificationUnread = false;
    }

    private async Task LoadActiveGroupPaymentSummaryAsync( int userId )
    {
        ResetActiveGroupPaymentSummary();

        var groupPayments = await _api.GetGroupPaymentsForUserAsync( userId, CancellationToken.None );
        var activeGroupPayment = groupPayments.FirstOrDefault();

        if (activeGroupPayment is null)
            return;

        HasActiveGroupPayment = true;
        ActiveOrderTitle = string.IsNullOrWhiteSpace( activeGroupPayment.Title )
            ? "Aktiv gruppebetaling"
            : activeGroupPayment.Title;

        ActiveMerchantName = activeGroupPayment.Members
            .FirstOrDefault( x => x.Type == DirectoryEntryType.Merchant )?.DisplayName
            ?? "Merchant";

        ActiveOrderIconSource = BuildIconSource( activeGroupPayment.IconKey );
        ActiveOrderActionText = "Åbn";

        var participants = activeGroupPayment.Members
            .Where( x => x.Type != DirectoryEntryType.Merchant )
            .ToList();

        TotalMembersCount = participants.Count;

        var latestOrders = await _api.GetLatestOrdersForGroupPaymentAsync( activeGroupPayment.Id, CancellationToken.None );
        var paidMemberIds = latestOrders
            .Where( x => x.LatestOrder is not null &&
                         x.LatestOrder.Lines is not null &&
                         x.LatestOrder.Lines.Count > 0 )
            .Select( x => x.MemberId )
            .Distinct()
            .ToHashSet();

        PaidMembersCount = participants.Count == 0
            ? 0
            : participants.Count( x => paidMemberIds.Contains( x.Id ) );

        ActiveParticipants.Clear();
        foreach (var participant in participants)
        {
            ActiveParticipants.Add(
                new HomeParticipantChipVm(
                    participant.Id,
                    participant.DisplayName,
                    paidMemberIds.Contains( participant.Id ) ) );
        }

        NotificationTitle = ActiveOrderTitle;
        NotificationSubtitle = "Ny besked om aktiv ordre";
        IsNotificationUnread = true;

        NotifyNotificationStateChanged();
        OnPropertyChanged( nameof( GreetingName ) );
    }

    private void ResetActiveGroupPaymentSummary()
    {
        HasActiveGroupPayment = false;
        ActiveOrderTitle = "Ingen aktiv ordre";
        ActiveMerchantName = "Åbn overblik for at se dine gruppebetalinger";
        ActiveOrderProgressText = "";
        ActiveOrderInfoText = "";
        ActiveOrderActionText = "Åbn";
        ActiveOrderIconSource = "icon_users";
        PaidMembersCount = 0;
        TotalMembersCount = 0;
        NotificationTitle = "Ingen notifikationer";
        NotificationSubtitle = "Log ind for at se dine notifikationer";
        IsNotificationUnread = false;
        ActiveParticipants.Clear();

        NotifyNotificationStateChanged();
    }

    partial void OnIsNotificationUnreadChanged( bool value )
    {
        NotifyNotificationStateChanged();
    }

    private void NotifyNotificationStateChanged()
    {
        OnPropertyChanged( nameof( NotificationCardBackground ) );
        OnPropertyChanged( nameof( NotificationCardStroke ) );
        OnPropertyChanged( nameof( NotificationCardStrokeThickness ) );
        OnPropertyChanged( nameof( NotificationSubtitleColor ) );
        OnPropertyChanged( nameof( NotificationTitleAttributes ) );
    }

    private static string BuildIconSource( string? iconKey )
    {
        var key = (iconKey ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace( key ))
            return "icon_users";

        if (!key.StartsWith( "icon_", StringComparison.OrdinalIgnoreCase ))
            key = $"icon_{key}";

        return key;
    }
}

public partial class HomeParticipantChipVm : ObservableObject
{
    public string Initials { get; }
    public Color FillColor { get; }
    public Color StrokeColor { get; }
    public bool HasPaid { get; }

    public HomeParticipantChipVm( int id, string? displayName, bool hasPaid )
    {
        Initials = CreateInitials( displayName );
        FillColor = CreateAvatarColor( id );
        StrokeColor = Colors.White;
        HasPaid = hasPaid;
    }

    private static string CreateInitials( string? displayName )
    {
        var parts = (displayName ?? string.Empty)
            .Trim()
            .Split( ' ', StringSplitOptions.RemoveEmptyEntries );

        if (parts.Length == 0) return "?";
        if (parts.Length == 1)
            return parts[ 0 ].Length >= 2
                ? parts[ 0 ][ ..2 ].ToUpperInvariant()
                : parts[ 0 ].ToUpperInvariant();

        return (parts[ 0 ][ 0 ].ToString() + parts[ ^1 ][ 0 ].ToString()).ToUpperInvariant();
    }

    private static Color CreateAvatarColor( int userId )
    {
        int r = (userId * 53) % 120 + 70;
        int g = (userId * 97) % 120 + 70;
        int b = (userId * 193) % 120 + 70;
        return Color.FromRgb( r, g, b );
    }
}