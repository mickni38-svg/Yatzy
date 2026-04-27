using BatchPay.Contracts.Dto;
using BatchPay.Frontend.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;


namespace BatchPay.Frontend.ViewModels;

public partial class CreateGroupPaymentViewModel : ObservableObject
{
    private readonly BatchPayApiClient _api;

    // Midlertidigt (til login-kobling): brug 1
    public int CurrentUserId { get; } = 1;

    // Midlertidigt: vælg en merchant (skal senere komme fra UI / DB)
    public int CurrentMerchantId { get; } = 1;

    public ObservableCollection<IconOptionVm> Icons { get; } = new();

    public ObservableCollection<UserDto> Friends { get; } = new();
    public ObservableCollection<FriendRowVm> FilteredFriends { get; } = new();

    private readonly HashSet<int> _selectedFriendIds = new();

    [ObservableProperty] private string friendSearchText = "";
    [ObservableProperty] private string title = "";
    [ObservableProperty] private string message = "";
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? statusMessage;

    public string SelectedIconKey => Icons.FirstOrDefault( x => x.IsSelected )?.Key ?? "other";
    public IEnumerable<int> SelectedMemberUserIds => _selectedFriendIds;

    public CreateGroupPaymentViewModel( BatchPayApiClient api )
    {
        _api = api;

        var keys = new[]
        {
            "pizza","beer","trip","party","work","coffee","burger","sushi","taco","music","movie",
            "food","cafe","education","fun","gift","health","housing","other","shopping","transport","travel","utilities"
        };

        foreach (var key in keys)
            Icons.Add( new IconOptionVm( key, $"icon_{key}.svg" ) );

        var first = Icons.FirstOrDefault();
        if (first != null) first.IsSelected = true;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            StatusMessage = null;

            Friends.Clear();
            FilteredFriends.Clear();

            var friends = await _api.GetFriendsAsync( CurrentUserId, CancellationToken.None );
            foreach (var f in friends)
            {
                Friends.Add( f );
                FilteredFriends.Add( new FriendRowVm( f, false ) );
            }
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

    partial void OnFriendSearchTextChanged( string value )
    {
        var q = (value ?? "").Trim().ToLowerInvariant();
        FilteredFriends.Clear();

        foreach (var f in Friends)
        {
            var match =
                string.IsNullOrWhiteSpace( q ) ||
                (f.DisplayName ?? "").ToLowerInvariant().Contains( q );

            if (!match) continue;

            var isSelected = _selectedFriendIds.Contains( f.Id );
            FilteredFriends.Add( new FriendRowVm( f, isSelected ) );
        }
    }

    [RelayCommand]
    private void ToggleFriend( FriendRowVm row )
    {
        if (row is null) return;

        if (_selectedFriendIds.Contains( row.UserId ))
        {
            _selectedFriendIds.Remove( row.UserId );
            row.IsSelected = false;
        }
        else
        {
            _selectedFriendIds.Add( row.UserId );
            row.IsSelected = true;
        }
    }

    [RelayCommand]
    private void SelectIcon( IconOptionVm icon )
    {
        if (icon is null) return;

        foreach (var i in Icons)
            i.IsSelected = false;

        icon.IsSelected = true;
    }

    [RelayCommand]
    private async Task CreateAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            StatusMessage = null;

            if (string.IsNullOrWhiteSpace( Title ))
            {
                StatusMessage = "Titel mangler.";
                return;
            }

            // ✅ VIGTIGT: DTO skal have MerchantId + MemberIds
            var request = new CreateGroupPaymentRequestDto(
                CreatedByUserId: CurrentUserId,
                MerchantId: CurrentMerchantId,
                Title: Title.Trim(),
                Message: Message?.Trim(),
                IconKey: SelectedIconKey,
                IsActive: true,
                CreatedAtUtc: DateTime.UtcNow,
                MemberIds: SelectedMemberUserIds.ToList()
            );

            await _api.CreateGroupPaymentAsync( request, CancellationToken.None );

            await Shell.Current.DisplayAlert( "Oprettet", "Bestilling oprettet", "OK" );
            await Shell.Current.GoToAsync( "//overview" );
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
}

public partial class FriendRowVm : ObservableObject
{
    public int UserId { get; }
    public string Name { get; }
    public string Initials { get; }

    [ObservableProperty] private bool isSelected;

    public FriendRowVm( UserDto user, bool isSelected )
    {
        UserId = user.Id;
        Name = user.DisplayName ?? $"User {user.Id}";
        Initials = GetInitials( Name );
        IsSelected = isSelected;
    }

    private static string GetInitials( string name )
    {
        var parts = (name ?? "").Trim().Split( ' ', StringSplitOptions.RemoveEmptyEntries );
        if (parts.Length == 0) return "?";
        if (parts.Length == 1) return parts[ 0 ].Length >= 2 ? parts[ 0 ][ ..2 ].ToUpperInvariant() : parts[ 0 ].ToUpperInvariant();
        return (parts[ 0 ][ 0 ].ToString() + parts[ 1 ][ 0 ].ToString()).ToUpperInvariant();
    }
}

public partial class IconOptionVm : ObservableObject
{
    public string Key { get; }
    public string ImageSource { get; }

    [ObservableProperty] private bool isSelected;

    public IconOptionVm( string key, string imageSource )
    {
        Key = key;
        ImageSource = imageSource;
    }
}