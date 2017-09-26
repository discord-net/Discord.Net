using Discord;
using Discord.Commands;
using Discord.WebSocket;
using foxboat.Services;

public class Commands
{
    public async Task Install(DiscordSocketClient client)
    {
        // Here, we will inject the ServiceProvider with 
        // all of the services our client will use.
        _serviceCollection.AddSingleton(client);
        _serviceCollection.AddSingleton(new NotificationService());
        _serviceCollection.AddSingleton(new DatabaseService());
        // ...
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
    }
}
