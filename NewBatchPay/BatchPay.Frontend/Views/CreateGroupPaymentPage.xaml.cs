using System.ComponentModel;
using BatchPay.Frontend.ViewModels;

namespace BatchPay.Frontend.Views;

public partial class CreateGroupPaymentPage : ContentPage
{
    private readonly CreateGroupViewModel _vm;

    public CreateGroupPaymentPage( CreateGroupViewModel vm )
    {
        InitializeComponent();

        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _vm.PropertyChanged += OnVmPropertyChanged;

        // Brug helst async-metoden direkte (mere robust end Command.Execute)
        await _vm.LoadAsync();
    }

    protected override void OnDisappearing()
    {
        _vm.PropertyChanged -= OnVmPropertyChanged;
        base.OnDisappearing();
    }

    private void OnVmPropertyChanged( object? sender, PropertyChangedEventArgs e )
    {
        if (e.PropertyName == nameof( CreateGroupViewModel.StatusMessage ))
        {
            if (string.Equals( _vm.StatusMessage, "Group created successfully!", StringComparison.Ordinal ))
            {
                _ = NavigateToOverviewAsync();
            }
        }
    }

    private async Task NavigateToOverviewAsync()
    {
        // Undgå dobbelt-navigation ved flere PropertyChanged events
        _vm.PropertyChanged -= OnVmPropertyChanged;

        // Hvis du bruger Shell:
        await Shell.Current.GoToAsync( "//overview" );
    }
}