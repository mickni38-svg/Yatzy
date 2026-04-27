using System;
using System.Linq;
using BatchPay.Contracts.Dto;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchPay.Frontend.Models;

public class SelectableDirectoryEntry : ObservableObject
{
    public string IconFileName { get; init; }

    public ImageSource IconImage => ImageSource.FromFile( IconFileName );
    public DirectoryEntryType Type { get; }
    public int Id { get; }

    public string DisplayName { get; }
    public string Handle { get; }
    public string? Subtitle { get; }
    public string? LogoUrl { get; }

    public bool IsMerchant => Type == DirectoryEntryType.Merchant;
    public bool IsUser => Type == DirectoryEntryType.User;

    /// <summary>
    /// I BatchPay kan både Users og Merchants vælges i "Find".
    /// </summary>
    public bool IsSelectable => true;

    // The [ObservableProperty] attribute has been removed.
    // The IsSelected property is now implemented manually below.
    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty( ref _isSelected, value );
    }

    public string Initials { get; }
    public string AvatarColor { get; }

    public SelectableDirectoryEntry( DirectoryEntryDto dto )
    {
        Type = dto.Type;
        Id = dto.Id;
        DisplayName = dto.DisplayName;
        Handle = dto.Handle;
        Subtitle = dto.Subtitle;
        LogoUrl = dto.LogoUrl;

        IconFileName = IsMerchant ? "icon_store.png" : "icon_user.png";

        Initials = BuildInitials( DisplayName );
        AvatarColor = IsMerchant ? "#1C2B22" : StableAvatarColor( Id, Handle );
    }

    private static string BuildInitials( string name )
    {
        var parts = (name ?? "")
            .Split( new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries )
            .Take( 2 )
            .Select( p => char.ToUpperInvariant( p[ 0 ] ).ToString() )
            .ToArray();

        return parts.Length == 0 ? "?" : string.Concat( parts );
    }

    private static string StableAvatarColor( int id, string handle )
    {
        // simple stabil farve baseret på id+handle (samme input => samme farve)
        var seed = $"{id}:{handle}".GetHashCode();

        // Palet der passer til din mørke UI
        string[] palette =
        {
            "#2F5D8A", "#5B3E8E", "#2E6B57", "#8A5D2F", "#7A2F5B", "#3A6D8C"
        };

        var idx = Math.Abs( seed ) % palette.Length;
        return palette[ idx ];
    }
}
