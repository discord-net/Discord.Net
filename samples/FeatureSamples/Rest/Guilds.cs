using Discord;
using Discord.Models.Json;
using Discord.Rest;
using System.Text.Json;

namespace FeatureSamples;

public class Guilds
{
    public static async Task RunAsync(DiscordRestClient client)
    {
        await client.Guilds[915079505557721090].Invites["abc"].DeleteAsync();

        var result = await client.Guilds[123].Members[123].FetchAsync();
    }
}
