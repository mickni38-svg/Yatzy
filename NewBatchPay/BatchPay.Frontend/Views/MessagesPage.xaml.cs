using System.Runtime.Versioning;
using BatchPay.Frontend.ViewModels;

namespace BatchPay.Frontend.Views;

public partial class MessagesPage : ContentPage
{
    public MessagesPage( MessagesViewModel vm )
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // IMPORTANT:
        // Når man klikker "Oversigt" (TabBar), vil vi reloade for at hente nye data.
        // VM har IsBusy-guard, så vi ikke spammer API.
        if (BindingContext is MessagesViewModel vm)
            await vm.LoadAsync();
    }
}