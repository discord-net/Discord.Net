using Discord;
using Discord.Models.Json;
using Discord.Rest;
using System.Text.Json;

namespace FeatureSamples;

public class Guilds
{
    public static async Task RunAsync(DiscordRestClient client)
    {
        var member = client.Guilds[915079505557721090].Members[259053800755691520];

        Console.WriteLine(member?.Id);
    }
}
