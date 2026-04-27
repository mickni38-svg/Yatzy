using BatchPay.Data;
using BatchPay.Data.Seed;
using BatchPay.Logic.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder( args );

// Kør API på en stabil port (så MAUI emulator kan ramme den på 10.0.2.2:5000)
builder.WebHost.UseUrls( "http://0.0.0.0:5000" );
// Controllers
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c =>
{
    c.SwaggerDoc( "v1", new OpenApiInfo
    {
        Title = "BatchPay API",
        Version = "v1",
        Description = "API til brugere, venner og gruppebetalinger"
    } );
} );

// EF Core (DI DbContext)
builder.Services.AddDbContext<BatchPayContext>( opt =>
{
    var cs = builder.Configuration.GetConnectionString( "BatchPayDb" );
    opt.UseSqlServer( cs, sql =>
    {
        // Migrations ligger i BatchPay.Data
        sql.MigrationsAssembly( "BatchPay.Data" );
    } );
} );

// Logic services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFriendService, FriendService>();
builder.Services.AddScoped<IGroupPaymentService, GroupPaymentService>();
builder.Services.AddScoped<IDirectoryService, DirectoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Seed
builder.Services.AddScoped<DatabaseSeeder>();


var app = builder.Build();

// Swagger (jeg lader den være tændt hele tiden mens vi udvikler)
app.UseSwagger();
app.UseSwaggerUI( c =>
{
    c.SwaggerEndpoint( "/swagger/v1/swagger.json", "BatchPay API v1" );
    c.RoutePrefix = "swagger";
} );

// Routing + Controllers
app.UseRouting();
app.UseAuthorization();
app.MapControllers();


// Migrate + Seed ved opstart (DI korrekt)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BatchPayContext>();
    await db.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedUsersAsync( CancellationToken.None );
    await seeder.SeedMerchantsAsync( CancellationToken.None );
}

app.Run();
