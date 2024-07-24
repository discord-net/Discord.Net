using Discord;
using Discord.Models;
using Discord.Rest;
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


    var guild = await client.Guilds.Specifically(915079505557721090).FetchAsync();


}
catch (Exception x)
{
    Console.WriteLine($"Failed: {x}");
}
