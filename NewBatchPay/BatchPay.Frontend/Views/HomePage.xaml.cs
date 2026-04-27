using System;
using BatchPay.Frontend.ViewModels;

namespace BatchPay.Frontend.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _vm;

    public HomePage( HomeViewModel vm )
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }

    private async void OnOverblikTapped( object sender, EventArgs e )
        => await Shell.Current.GoToAsync( "//overview" );

    private async void OnOpretTapped( object sender, EventArgs e )
        => await Shell.Current.GoToAsync( "//create" );

    private async void OnBrugereTapped( object sender, EventArgs e )
        => await Shell.Current.GoToAsync( "//find" );

    private async void OnBeskederTapped( object sender, EventArgs e )
    {
        _vm.MarkNotificationAsRead();
        await Shell.Current.GoToAsync( "//messages" );
    }

    private async void OnSignupTapped( object sender, EventArgs e )
        => await DisplayAlert( "Opret dig", "Opret dig som bruger kommer senere 🙂", "OK" );
}