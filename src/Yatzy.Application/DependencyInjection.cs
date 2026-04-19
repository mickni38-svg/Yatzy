using Microsoft.Extensions.DependencyInjection;
using Yatzy.Application.Interfaces;
using Yatzy.Application.Services;

namespace Yatzy.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IGameAppService, GameAppService>();
        services.AddScoped<IGameplayAppService, GameplayAppService>();
        services.AddSingleton<IConnectionService, ConnectionService>();
        return services;
    }
}
