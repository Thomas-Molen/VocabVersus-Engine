using vocabversus_engine.Hubs.GameHub;
using vocabversus_engine.Services;
using vocabversus_engine.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to DI
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

builder.Services.AddSingleton<IGameInstanceCache, GameInstanceCache>();
builder.Services.AddSingleton<IPlayerConnectionCache, PlayerConnectionCache>();
builder.Services.AddScoped<IWordSetService, WordSetService>();

// Add Http clients to DI
builder.Services.AddHttpClient<IWordSetService, WordSetService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7124/api/WordSet");
});

var app = builder.Build();

app.UseCors(builder =>
{
    builder.AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed((host) => true)
        .AllowCredentials();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<GameHub>("/game");

app.Run();
