using BatchPay.Frontend.ViewModels;

namespace BatchPay.Frontend.Views;

public partial class FindPersonsPage : ContentPage
{
    private readonly FindPersonsViewModel _vm;

    public FindPersonsPage( FindPersonsViewModel vm )
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
