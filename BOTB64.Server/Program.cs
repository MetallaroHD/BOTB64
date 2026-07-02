using BOTB64.Server.Endpoints;
using BOTB64.Server.Lobbies;
using BOTB64.Server.Matchmaking;
using BOTB64.Server.Relay;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<LobbyManager>();
builder.Services.AddSingleton<MatchmakingQueue>();
builder.Services.AddSingleton<MatchRelay>();
builder.Services.AddHostedService<RelayHostedService>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

var app = builder.Build();
app.Urls.Add("http://0.0.0.0:5000");

app.MapLobbyEndpoints();
app.MapMatchmakingEndpoints();

app.Run();