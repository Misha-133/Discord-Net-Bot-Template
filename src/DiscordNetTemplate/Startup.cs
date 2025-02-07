using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;

using DiscordNetTemplate.Options;
using DiscordNetTemplate.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;


var builder = new HostApplicationBuilder(args);

builder.Configuration
    .AddEnvironmentVariables("DOTNET_")
    .AddEnvironmentVariables("DNetTemplate_");

// map config section to options class
builder.Services.Configure<DiscordBotOptions>(builder.Configuration.GetSection("Discord"));


var loggerConfig = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File($"logs/log-{DateTime.Now:yy.MM.dd_HH.mm}.log")
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(loggerConfig, dispose: true);


builder.Services.AddSingleton(new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.AllUnprivileged,
    FormatUsersInBidirectionalUnicode = false,
    // Add GatewayIntents.GuildMembers to the GatewayIntents and change this to true if you want to download all users on startup
    AlwaysDownloadUsers = false,
    LogGatewayIntentWarnings = false,
    LogLevel = LogSeverity.Info
});

builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton<IRestClientProvider>(x => x.GetRequiredService<DiscordSocketClient>());

builder.Services.AddSingleton(new InteractionServiceConfig
{
    LogLevel = LogSeverity.Info,
    UseCompiledLambda = true
});

builder.Services.AddSingleton<InteractionService>();

builder.Services.AddHostedService<DiscordBotService>();
builder.Services.AddHostedService<InteractionHandler>();

var app = builder.Build();

await app.RunAsync();
