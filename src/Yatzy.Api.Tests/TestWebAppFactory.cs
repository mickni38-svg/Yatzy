using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Yatzy.Persistence;

namespace Yatzy.Api.Tests;

public sealed class TestWebAppFactory : WebApplicationFactory<Program>
{
    // Fixed name so every DbContext scope within this factory shares the same store
    private readonly string _dbName = "YatzyTestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // EF Core 9 registers multiple descriptor types — remove them all
            var toRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<YatzyDbContext>) ||
                    d.ServiceType == typeof(YatzyDbContext) ||
                    d.ServiceType == typeof(IDbContextOptionsConfiguration<YatzyDbContext>))
                .ToList();

            foreach (var d in toRemove)
                services.Remove(d);

            // All requests in this factory share the same in-memory database
            services.AddDbContext<YatzyDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));
        });

        builder.UseEnvironment("Development");
    }
}
