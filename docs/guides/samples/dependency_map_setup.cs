using Discord;
using Discord.Commands;
using Discord.WebSocket;
using foxboat.Services;

public class Commands
{
    public async Task Install(DiscordSocketClient client)
    {
        // Here, we will inject the Dependency Map with 
        // all of the services our client will use.
        _map.Add(client);
        _map.Add(commands);
        _map.Add(new NotificationService(_map));
        _map.Add(new DatabaseService(_map));
        // ...
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
    }
}
