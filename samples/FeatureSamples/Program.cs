using Discord;
using Discord.Models;
using Discord.Rest;
using FeatureSamples;
using System.Runtime.InteropServices;
using System.Text.Json;

try
{
    var token = Environment.GetEnvironmentVariable("TOKEN");

    if (token is null)
    {
        Console.WriteLine("Missing TOKEN environment variable.");
        return;
    }

    var client = new DiscordRestClient(token);

    await Guilds.RunAsync(client);
}
catch (Exception x)
{
    Console.WriteLine($"Failed: {x}");
}
