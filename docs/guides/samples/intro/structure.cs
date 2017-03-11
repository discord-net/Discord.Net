using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

class Program
{
    private readonly DiscordSocketClient _client;
    
    // Keep the CommandService and IDependencyMap around for use with commands.
    private readonly IDependencyMap _map = new DependencyMap();
    private readonly CommandService _commands = new CommandService();

    // Program entry point
    static void Main(string[] args)
    {
        // Call the Program constructor, followed by the 
        // MainAsync method and wait until it finishes (which should be never).
        new Program().MainAsync().GetAwaiter().GetResult();
    }

    private Program()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            // How much logging do you want to see?
            LogLevel = LogSeverity.Info,
            
            // If you or another service needs to do anything with messages
            // (eg. checking Reactions), you should probably
            // set the MessageCacheSize here.
            //MessageCacheSize = 50,

            // If your platform doesn't have native websockets,
            // add Discord.Net.Providers.WS4Net from NuGet,
            // add the `using` at the top, and uncomment this line:
            //WebSocketProvider = WS4NetProvider.Instance
        });
    }

    // Create a named logging handler, so it can be re-used by addons
    // that ask for a Func<LogMessage, Task>.
    private static Task Logger(LogMessage message)
    {
        var cc = Console.ForegroundColor;
        switch (message.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogSeverity.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogSeverity.Info:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogSeverity.Verbose:
            case LogSeverity.Debug:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
        }
        Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message}");
        Console.ForegroundColor = cc;
        return Task.CompletedTask;
    }

    private async Task MainAsync()
    {
        // Subscribe the logging handler.
        _client.Log += Logger;

        // Centralize the logic for commands into a seperate method.
        await InitCommands();

        // Login and connect.
        await _client.LoginAsync(TokenType.Bot, /* <DON'T HARDCODE YOUR TOKEN> */);
        await _client.StartAsync();
        
        // Wait infinitely so your bot actually stays connected.
        await Task.Delay(-1);
    }

    private async Task InitCommands()
    {
        // Repeat this for all the service classes
        // and other dependencies that your commands might need.
        _map.Add(new SomeServiceClass());

        // Either search the program and add all Module classes that can be found:
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        // Or add Modules manually if you prefer to be a little more explicit:
        await _commands.AddModuleAsync<SomeModule>();

        // Subscribe a handler to see if a message invokes a command.
        _client.MessageReceived += HandleCommandAsync;
    }

    private async Task HandleCommandAsync(SocketMessage arg)
    {
        // Bail out if it's a System Message.
        var msg = arg as SocketUserMessage;
        if (msg == null) return;

        // Create a number to track where the prefix ends and the command begins
        int pos = 0;
        // Replace the '!' with whatever character
        // you want to prefix your commands with.
        // Uncomment the second half if you also want
        // commands to be invoked by mentioning the bot instead.
        if (msg.HasCharPrefix('!', ref pos) /* || msg.HasMentionPrefix(msg.Discord.CurrentUser, ref pos) */)
        {
            // Create a Command Context
            var context = new SocketCommandContext(msg.Discord, msg);
            
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed succesfully).
            var result = await _commands.ExecuteAsync(context, pos, _map);

            // Uncomment the following lines if you want the bot
            // to send a message if it failed (not advised for most situations).
            //if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            //    await msg.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}
