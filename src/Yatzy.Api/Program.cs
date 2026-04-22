using Yatzy.Application;
using Yatzy.Infrastructure;
using Yatzy.Persistence;
using Yatzy.Api.Middleware;
using Yatzy.Api.Hubs;
using Yatzy.Api.Services;
using Yatzy.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ændret: CORS tillader kun localhost i Development – i Production serves Angular fra wwwroot
builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            // I produktion kommer Angular fra samme origin som API'et
            policy.WithOrigins("https://paybysharepay.dk", "https://www.paybysharepay.dk")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});

builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
        options.PayloadSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddScoped<IGameHubService, GameHubService>();

var app = builder.Build();

// Kør database migrationer automatisk ved opstart
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<YatzyDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("Angular");

// Tilføjet: server Angular-filer fra wwwroot i produktion
app.UseDefaultFiles();   // index.html som default
app.UseStaticFiles();    // wwwroot/**

app.UseAuthorization();
app.MapControllers();
app.MapHub<GameHub>("/hubs/game");
app.MapHub<VideoHub>("/hubs/video");

// Tilføjet: SPA fallback – alle ikke-API ruter sender index.html til Angular-routeren
app.MapFallbackToFile("index.html");

app.Run();

// Expose Program for WebApplicationFactory in integration tests
public partial class Program { }

