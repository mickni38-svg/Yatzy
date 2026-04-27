using BatchPay.Frontend.ViewModels;

namespace BatchPay.Frontend.Views;

public partial class OverviewPage : ContentPage
{
    public OverviewPage( OverviewViewModel vm )
    {
        InitializeComponent();

        // IMPORTANT: DI injicerer ViewModel som BindingContext
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // IMPORTANT:
        // Når man klikker "Oversigt" (TabBar), vil vi reloade for at hente nye data.
        // VM har IsBusy-guard, så vi ikke spammer API.
        if (BindingContext is OverviewViewModel vm)
            await vm.LoadAsync();
    }
}
