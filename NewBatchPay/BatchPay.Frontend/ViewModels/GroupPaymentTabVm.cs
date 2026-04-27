using BatchPay.Contracts.Dto;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchPay.Frontend.ViewModels;

public partial class GroupPaymentTabVm : ObservableObject
{
    public GroupPaymentDto Model { get; }

    public string Title => Model.Title;

    public string MerchantDisplayName { get; set; } = "hey";

    public string? IconKey => Model.IconKey;

    public string IconSource
    {
        get
        {
            var key = (IconKey ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace( key ))
            {
                return "icon_users";
            }

            if (!key.StartsWith( "icon_", StringComparison.OrdinalIgnoreCase ))
            {
                key = $"icon_{key}";
            }

            // MAUI MauiImage er typisk mest stabil uden filendelse i XAML
            if (key.EndsWith( ".svg", StringComparison.OrdinalIgnoreCase ))
            {
                key = key[ ..^4 ];
            }

            return key;
        }
    }

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private bool showDivider = true;

    public GroupPaymentTabVm( GroupPaymentDto model )
    {
        Model = model;
        MerchantDisplayName =
            model.Members.FirstOrDefault( x => x.Type == DirectoryEntryType.Merchant )?.DisplayName
            ?? string.Empty;
    }
}