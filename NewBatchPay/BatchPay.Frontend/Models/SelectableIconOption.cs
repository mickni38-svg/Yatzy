using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchPay.Frontend.Models;

public sealed partial class SelectableIconOption : ObservableObject
{
    public string IconKey { get; }

    [ObservableProperty]
    private bool isSelected;

    public SelectableIconOption( string iconKey )
        => IconKey = iconKey;
}