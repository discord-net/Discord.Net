using Discord;
using Discord.Gateway;
using Discord.Models;
using Discord.Rest;
using FeatureSamples;
using FeatureSamples.Gateway;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Serilog;

using var loggerFactory = LoggerFactory.Create(builder => builder
    .AddSerilog(
        new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Verbose()
            .WriteTo.Console(
                outputTemplate: "{Timestamp:HH:mm:ss} | {Level} - [{SourceContext}]: {Message:lj}{NewLine}{Exception}")
            .CreateLogger()
    )
    .SetMinimumLevel(LogLevel.Trace)
);

var token = Environment.GetEnvironmentVariable("TOKEN");

if (token is null)
{
    Console.WriteLine("Missing TOKEN environment variable.");
    return;
}

var restClient = new DiscordRestClient(token);
var gatewayClient = new DiscordGatewayClient(
    new DiscordGatewayConfig(token)
    {
        TransportCompression = null,
        Intents = GatewayIntents.Guilds
    }, 
    loggerFactory
);

await GatewayEvents.RunAsync(gatewayClient);
//await Guilds.RunAsync(restClient);

await Task.Delay(-1);