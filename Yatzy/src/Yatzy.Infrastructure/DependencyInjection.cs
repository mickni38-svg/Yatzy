using Microsoft.Extensions.DependencyInjection;
using Yatzy.Domain.Interfaces;
using Yatzy.Domain.Rules;

namespace Yatzy.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IRandomProvider, RandomProvider>();
        services.AddSingleton<IScoreCalculator, ScoreCalculator>();
        return services;
    }
}
