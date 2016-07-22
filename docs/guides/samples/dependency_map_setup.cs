using Discord;
using Discord.Commands;

public class Commands
{
    public async Task Install(DiscordSocketClient client)
    {
        var commands = new CommandService();
        var map = new DependencyMap();
        map.Add<IDiscordClient>(client);
        var self = await client.GetCurrentUserAsync();
        map.Add<ISelfUser>(self);
        await commands.LoadAssembly(Assembly.GetCurrentAssembly(), map);
    }
    // ...
}