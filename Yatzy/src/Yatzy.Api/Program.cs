using Yatzy.Application;
using Yatzy.Infrastructure;
using Yatzy.Persistence;
using Yatzy.Api.Middleware;
using Yatzy.Api.Hubs;
using Yatzy.Api.Services;
using Yatzy.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("Angular");
app.UseAuthorization();
app.MapControllers();
app.MapHub<GameHub>("/hubs/game");
app.MapHub<VideoHub>("/hubs/video");
app.Run();

// Expose Program for WebApplicationFactory in integration tests
public partial class Program { }

