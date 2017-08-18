using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

class Program
{
    // Program entry point
    static void Main(string[] args)
    {
        // Call the Program constructor, followed by the 
        // MainAsync method and wait until it finishes (which should be never).
        new Program().MainAsync().GetAwaiter().GetResult();
    }

    private readonly DiscordSocketClient _client;
    
    // Keep the CommandService and IServiceCollection around for use with commands.
    // These two types require you install the Discord.Net.Commands package.
    private readonly IServiceCollection _map = new ServiceCollection();
    private readonly CommandService _commands = new CommandService();

    private Program()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            // How much logging do you want to see?
            LogLevel = LogSeverity.Info,
            
            // If you or another service needs to do anything with messages
            // (eg. checking Reactions, checking the content of edited/deleted messages),
            // you must set the MessageCacheSize. You may adjust the number as needed.
            //MessageCacheSize = 50,

            // If your platform doesn't have native websockets,
            // add Discord.Net.Providers.WS4Net from NuGet,
            // add the `using` at the top, and uncomment this line:
            //WebSocketProvider = WS4NetProvider.Instance
        });
        // Subscribe the logging handler to both the client and the CommandService.
        _client.Log += Logger;
        _commands.Log += Logger;
    }

    // Example of a logging handler. This can be re-used by addons
    // that ask for a Func<LogMessage, Task>.
    private static Task Logger(LogMessage message)
    {
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
        Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
        Console.ResetColor();
        
        // If you get an error saying 'CompletedTask' doesn't exist,
        // your project is targeting .NET 4.5.2 or lower. You'll need
        // to adjust your project's target framework to 4.6 or higher
        // (instructions for this are easily Googled).
        // If you *need* to run on .NET 4.5 for compat/other reasons,
        // the alternative is to 'return Task.Delay(0);' instead.
        return Task.CompletedTask;
    }

    private async Task MainAsync()
    {
        // Centralize the logic for commands into a seperate method.
        await InitCommands();

        // Login and connect.
        await _client.LoginAsync(TokenType.Bot, /* <DON'T HARDCODE YOUR TOKEN> */);
        await _client.StartAsync();

        // Wait infinitely so your bot actually stays connected.
        await Task.Delay(-1);
    }

    private IServiceProvider _services;
    
    private async Task InitCommands()
    {
        // Repeat this for all the service classes
        // and other dependencies that your commands might need.
        _map.AddSingleton(new SomeServiceClass());

        // When all your required services are in the collection, build the container.
        // Tip: There's an overload taking in a 'validateScopes' bool to make sure
        // you haven't made any mistakes in your dependency graph.
        _services = _map.BuildServiceProvider();

        // Either search the program and add all Module classes that can be found.
        // Module classes MUST be marked 'public' or they will be ignored.
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        // Or add Modules manually if you prefer to be a little more explicit:
        await _commands.AddModuleAsync<SomeModule>();
        // Note that the first one is 'Modules' (plural) and the second is 'Module' (singular).

        // Subscribe a handler to see if a message invokes a command.
        _client.MessageReceived += HandleCommandAsync;
    }

    private async Task HandleCommandAsync(SocketMessage arg)
    {
        // Bail out if it's a System Message.
        var msg = arg as SocketUserMessage;
        if (msg == null) return;

        // We don't want the bot to respond to itself or other bots.
        // NOTE: Selfbots should invert this first check and remove the second
        // as they should ONLY be allowed to respond to messages from the same account.
        if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;
        
        // Create a number to track where the prefix ends and the command begins
        int pos = 0;
        // Replace the '!' with whatever character
        // you want to prefix your commands with.
        // Uncomment the second half if you also want
        // commands to be invoked by mentioning the bot instead.
        if (msg.HasCharPrefix('!', ref pos) /* || msg.HasMentionPrefix(_client.CurrentUser, ref pos) */)
        {
            // Create a Command Context.
            var context = new SocketCommandContext(_client, msg);
            
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed succesfully).
            var result = await _commands.ExecuteAsync(context, pos, _services);

            // Uncomment the following lines if you want the bot
            // to send a message if it failed (not advised for most situations).
            //if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            //    await msg.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}
