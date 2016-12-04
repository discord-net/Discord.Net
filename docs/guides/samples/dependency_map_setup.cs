using Discord;
using Discord.Commands;
using Discord.WebSocket;
using foxboat.Services;

public class Commands
{
    public async Task Install(DiscordSocketClient client)
    {
        var commands = new CommandService();
        var map = new DependencyMap();
        map.Add(client);
        map.Add(commands);
        await commands.AddModulesAsync(Assembly.GetEntryAssembly());
    }
    // In ConfigureServices, we will inject the Dependency Map with 
    // all of the services our client will use.
    public Task ConfigureServices(IDependencyMap map)
    {
        map.Add(new NotificationService(map));
        map.Add(new DatabaseService(map));
    }
    // ...
}
