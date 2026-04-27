using BatchPay.Contracts.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;

namespace BatchPay.Frontend.Models;

public partial class SelectableUser : ObservableObject
{
    public UserDto User { get; }

    [ObservableProperty]
    private bool isSelected;

    public SelectableUser( UserDto user )
    {
        User = user;
        isSelected = false; // Default: IKKE valgt

        Initials = CreateInitials( user.DisplayName );
        AvatarColor = CreateAvatarColor( user.Id );
    }

    public int Id => User.Id;
    public string DisplayName => User.DisplayName;
    public string Handle => User.Handle;

    // Used by FindPersonsPage (avatar)
    public string Initials { get; }
    public Color AvatarColor { get; }

    private static string CreateInitials( string displayName )
    {
        var parts = (displayName ?? "").Trim().Split( ' ', StringSplitOptions.RemoveEmptyEntries );

        if (parts.Length == 0) return "?";
        if (parts.Length == 1)
            return parts[ 0 ].Substring( 0, Math.Min( 2, parts[ 0 ].Length ) ).ToUpperInvariant();

        var first = parts[ 0 ].Substring( 0, 1 );
        var last = parts[ ^1 ].Substring( 0, 1 );
        return (first + last).ToUpperInvariant();
    }

    // Cooler/darker palette friendly for dark mode (still deterministic per user)
    private static Color CreateAvatarColor( int userId )
    {
        int r = (userId * 53) % 110 + 30;
        int g = (userId * 97) % 110 + 30;
        int b = (userId * 193) % 110 + 30;
        return Color.FromRgb( r, g, b );
    }
}
