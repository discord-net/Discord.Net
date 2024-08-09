using Discord;
using Discord.Gateway;
using Discord.Models;
using Discord.Rest;
using FeatureSamples;
using FeatureSamples.Gateway;
using System.Runtime.InteropServices;
using System.Text.Json;

var token = Environment.GetEnvironmentVariable("TOKEN");

if (token is null)
{
    Console.WriteLine("Missing TOKEN environment variable.");
    return;
}

var restClient = new DiscordRestClient(token);
var gatewayClient = new DiscordGatewayClient(token);

await GatewayEvents.RunAsync(gatewayClient);
await Guilds.RunAsync(restClient);
