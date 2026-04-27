using BatchPay.Frontend.DevTools;
using BatchPay.Frontend.Services;
using BatchPay.Frontend.ViewModels;
using BatchPay.Frontend.Views;
using System;

namespace BatchPay.Frontend;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.UseMauiApp<App>();

#if ANDROID
        var apiBase = "http://10.0.2.2:5000/";
#else
        var apiBase = "http://localhost:5000/";
#endif

#if DEBUG && !ANDROID && !IOS
        DevIconSeeder.EnsureIconsExist();
#endif

        builder.Services.AddSingleton( _ => new HttpClient { BaseAddress = new Uri( apiBase ) } );
        builder.Services.AddSingleton<BatchPayApiClient>();

        builder.Services.AddScoped<IUserContext, UserContext>();

        // ViewModels
        builder.Services.AddTransient<HomeViewModel>();            // ✅ FIX: Manglede
        builder.Services.AddTransient<FindPersonsViewModel>();
        builder.Services.AddTransient<CreateGroupViewModel>();
        builder.Services.AddTransient<OverviewViewModel>();
        builder.Services.AddTransient<CreateGroupPaymentViewModel>();
        builder.Services.AddTransient<MessagesViewModel>();

        // Pages
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<CreateGroupPaymentPage>();
        builder.Services.AddTransient<FindPersonsPage>();
        builder.Services.AddTransient<OverviewPage>();
        builder.Services.AddTransient<MessagesPage>();

        builder.Services.AddSingleton<AppShell>();

        return builder.Build();
    }
}