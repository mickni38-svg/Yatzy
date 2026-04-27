using BatchPay.Frontend.Views;

namespace BatchPay.Frontend;

public partial class AppShell : Shell
{
    public AppShell( IServiceProvider services )
    {
        InitializeComponent();

        // Resolve pages via DI (så de kan have ctor-parametre som ViewModels)
        HomeTab.Content = services.GetRequiredService<HomePage>();
        OverviewTab.Content = services.GetRequiredService<OverviewPage>();
        CreateTab.Content = services.GetRequiredService<CreateGroupPaymentPage>();
        FindTab.Content = services.GetRequiredService<FindPersonsPage>();
        MessagesTab.Content = services.GetRequiredService<MessagesPage>();

        Routing.RegisterRoute( nameof( HomePage ), typeof( HomePage ) );
        Routing.RegisterRoute( nameof( OverviewPage ), typeof( OverviewPage ) );
        Routing.RegisterRoute( nameof( CreateGroupPaymentPage ), typeof( CreateGroupPaymentPage ) );
        Routing.RegisterRoute( nameof( FindPersonsPage ), typeof( FindPersonsPage ) );
        Routing.RegisterRoute( nameof( MessagesPage ), typeof( MessagesPage ) );
    }
}