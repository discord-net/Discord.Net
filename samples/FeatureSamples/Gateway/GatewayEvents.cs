using Discord.Gateway;
using Discord.Models;

namespace FeatureSamples.Gateway;

public class GatewayEvents
{
    public static async Task RunAsync(DiscordGatewayClient client)
    {
        await client.ConnectAsync();
        
        client.GuildAvailable += ClientOnGuildAvailable;
        client.GuildCreated += ClientOnGuildCreated;
        client.Ready += ClientOnReady;
        
        // client.GatewayDisconnected += MyHandler;
        // client.GatewayConnected += MyHandler;
        // client.GatewayResumed += MyHandler;
        // client.Ready += MyHandler;
    }

    private static ValueTask ClientOnReady(GatewayCurrentUser currentUser, IReadOnlySet<ulong> guilds)
    {
        Console.WriteLine($"Ready! {currentUser.Username} | {guilds.Count} guilds");
        return ValueTask.CompletedTask;
    }

    private static ValueTask ClientOnGuildCreated(GatewayGuild guild)
    {
        Console.WriteLine($"Guild Created! {guild.Name}");
        return ValueTask.CompletedTask;
    }

    private static ValueTask ClientOnGuildAvailable(GatewayGuild guild)
    {
        Console.WriteLine($"Guild Available! {guild.Name}");
        return ValueTask.CompletedTask;
    }
}
