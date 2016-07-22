using Discord;
using Discord.Commands;

[Module]
public class ModuleA
{
    private DiscordSocketClient client;
    private ISelfUser self;

    public ModuleA(IDiscordClient c, ISelfUser s)
    {
        if (!(c is DiscordSocketClient)) throw new InvalidOperationException("This module requires a DiscordSocketClient");
        client = c as DiscordSocketClient;
        self = s;
    }
}

public class ModuleB
{
    private IDiscordClient client;
    private CommandService commands;
    
    public ModuleB(CommandService c, IDependencyMap m)
    {
        commands = c;
        client = m.Get<IDiscordClient>();
    }
}