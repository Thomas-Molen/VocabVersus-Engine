using vocabversus_engine.Hubs.GameHub;
using vocabversus_engine.Services;
using vocabversus_engine.Utility;
using vocabversus_engine.Utility.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to DI
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

builder.Services.AddSingleton<IGameInstanceCache, GameInstanceCache>();
builder.Services.AddSingleton<IPlayerConnectionCache, PlayerConnectionCache>();
builder.Services.AddScoped<IWordSetService, WordSetService>();

// Add configuration settings
var wordSetEvaluatorSettings = builder.Configuration.GetSection(WordSetEvaluatorSettings.SectionName);
builder.Services.Configure<WordSetEvaluatorSettings>(wordSetEvaluatorSettings);

// Add Http clients to DI
builder.Services.AddHttpClient<IWordSetService, WordSetService>(client =>
{
    client.BaseAddress = new Uri($"{wordSetEvaluatorSettings.Get<WordSetEvaluatorSettings>()?.BaseUrl ?? throw new ArgumentException("No BaseUrl for WordSetEvaluator was found")}/api/WordSet");
});


var app = builder.Build();

app.UseCors(builder =>
{
    builder.WithOrigins("https://vocabversus.azurewebsites.net")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed((host) => true)
        .AllowCredentials();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<GameHub>("/game");

app.Run();
