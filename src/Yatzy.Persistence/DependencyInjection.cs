using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yatzy.Application.Interfaces;
using Yatzy.Persistence.Repositories;

namespace Yatzy.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<YatzyDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(YatzyDbContext).Assembly.FullName)));

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
