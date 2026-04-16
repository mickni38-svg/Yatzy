using BatchPay.Contracts.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BatchPay.Frontend.ViewModels;

public partial class GroupPaymentMemberVm : ObservableObject
{
    public bool IsMerchant { get; }
    public int UserId { get; }
    public string Initials { get; }
    public Color AvatarColor { get; }
    public string DisplayName { get; set; }
    public string MerchantDisplayName { get; }

    [ObservableProperty] private bool isExpanded;

    public ObservableCollection<OrderLineVm> OrderLines { get; } = new();

    [ObservableProperty] private string? orderTitle;

    public bool HasOrder { get; set; }

    public string OrderTotalText
    {
        get
        {
            var total = OrderLines.Sum( x => x.Quantity * x.UnitPrice );
            return $"{total:0.00} kr";
        }
    }

    public string PaymentStatusText => HasOrder ? "Betalt" : "Afventer";

    // NYT: farver til UI
    public Color RowBackgroundColor => HasOrder
        ? Color.FromArgb( "#0B2B30" )
        : Color.FromArgb( "#071923" );

    public Color RowBorderColor => HasOrder
        ? Color.FromArgb( "#49DCCB" )
        : Color.FromArgb( "#173847" );

    public Color StatusBackgroundColor => HasOrder
        ? Color.FromArgb( "#173D3B" )
        : Color.FromArgb( "#102636" );

    public Color StatusTextColor => HasOrder
        ? Color.FromArgb( "#B9FFE2" )
        : Color.FromArgb( "#D7E5F2" );

    public Color NameTextColor => HasOrder
        ? Color.FromArgb( "#F2FFFB" )
        : Colors.White;

    public Color ChevronColor => HasOrder
        ? Color.FromArgb( "#8EF3D7" )
        : Color.FromArgb( "#D6E2F2" );

    public Color DetailsBackgroundColor => HasOrder
        ? Color.FromArgb( "#0C2128" )
        : Color.FromArgb( "#09131C" );

    public Color DetailsBorderColor => HasOrder
        ? Color.FromArgb( "#2E8F9E" )
        : Color.FromArgb( "#2A4357" );

    public GroupPaymentMemberVm( DirectoryEntryDto user )
    {
        IsMerchant = user.Type == DirectoryEntryType.Merchant;
        UserId = user.Id;
        Initials = CreateInitials( user.DisplayName );
        AvatarColor = CreateAvatarColor( user.Id );
        DisplayName = user.DisplayName;

        OrderLines.CollectionChanged += OnOrderLinesChanged;
        OrderTitle = null;
    }

    private void OnOrderLinesChanged( object? sender, NotifyCollectionChangedEventArgs e )
    {
        OnPropertyChanged( nameof( HasOrder ) );
        OnPropertyChanged( nameof( OrderTotalText ) );
        OnPropertyChanged( nameof( PaymentStatusText ) );
        OnPropertyChanged( nameof( RowBackgroundColor ) );
        OnPropertyChanged( nameof( RowBorderColor ) );
        OnPropertyChanged( nameof( StatusBackgroundColor ) );
        OnPropertyChanged( nameof( StatusTextColor ) );
        OnPropertyChanged( nameof( NameTextColor ) );
        OnPropertyChanged( nameof( ChevronColor ) );
        OnPropertyChanged( nameof( DetailsBackgroundColor ) );
        OnPropertyChanged( nameof( DetailsBorderColor ) );
    }

    public void ApplyLatestOrder( OrderDto? order )
    {
        if (order is null)
        {
            OrderLines.Clear();
            HasOrder = false;

            OnPropertyChanged( nameof( HasOrder ) );
            OnPropertyChanged( nameof( OrderTotalText ) );
            OnPropertyChanged( nameof( PaymentStatusText ) );
            OnPropertyChanged( nameof( RowBackgroundColor ) );
            OnPropertyChanged( nameof( RowBorderColor ) );
            OnPropertyChanged( nameof( StatusBackgroundColor ) );
            OnPropertyChanged( nameof( StatusTextColor ) );
            OnPropertyChanged( nameof( NameTextColor ) );
            OnPropertyChanged( nameof( ChevronColor ) );
            OnPropertyChanged( nameof( DetailsBackgroundColor ) );
            OnPropertyChanged( nameof( DetailsBorderColor ) );
            return;
        }

        OrderLines.Clear();

        foreach (var l in order.Lines)
            OrderLines.Add( new OrderLineVm( l.ItemName, l.Quantity, l.UnitPrice ) );

        HasOrder = OrderLines.Count > 0;

        OnPropertyChanged( nameof( HasOrder ) );
        OnPropertyChanged( nameof( OrderTotalText ) );
        OnPropertyChanged( nameof( PaymentStatusText ) );
        OnPropertyChanged( nameof( RowBackgroundColor ) );
        OnPropertyChanged( nameof( RowBorderColor ) );
        OnPropertyChanged( nameof( StatusBackgroundColor ) );
        OnPropertyChanged( nameof( StatusTextColor ) );
        OnPropertyChanged( nameof( NameTextColor ) );
        OnPropertyChanged( nameof( ChevronColor ) );
        OnPropertyChanged( nameof( DetailsBackgroundColor ) );
        OnPropertyChanged( nameof( DetailsBorderColor ) );
    }

    [RelayCommand]
    private void Toggle()
    {
        IsExpanded = !IsExpanded;
    }

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

    private static Color CreateAvatarColor( int userId )
    {
        int r = (userId * 53) % 120 + 40;
        int g = (userId * 97) % 120 + 40;
        int b = (userId * 193) % 120 + 40;
        return Color.FromRgb( r, g, b );
    }

    public string TabText => $"{OrderTitle} · {DisplayName}";
}

public sealed partial class OrderLineVm : ObservableObject
{
    public OrderLineVm( string itemName, int quantity, decimal unitPrice )
    {
        ItemName = itemName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public string ItemName { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }

    public string UnitPriceText => $"{UnitPrice:0.00} kr";
}