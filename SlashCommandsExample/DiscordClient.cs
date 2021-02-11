using Discord;
using Discord.Commands;
using Discord.SlashCommands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace SlashCommandsExample
{
    class DiscordClient
    {
        public static DiscordSocketClient socketClient { get; set; } = new DiscordSocketClient();
        public static SlashCommandService _commands { get; set; }
        public static IServiceProvider _services { get; set; }

        private string botToken = "<YOUR TOKEN HERE>";

        public DiscordClient()
        {
            _commands = new SlashCommandService();
            _services = new ServiceCollection()
                .AddSingleton(socketClient)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            socketClient.Log += SocketClient_Log;
            _commands.Log += SocketClient_Log;
            socketClient.InteractionCreated += InteractionHandler;
            // This is for dev purposes.
            // To avoid the situation in which you accidentally push your bot token to upstream, you can use
            // EnviromentVariables to store your key. 
            botToken = Environment.GetEnvironmentVariable("DiscordSlashCommandsBotToken", EnvironmentVariableTarget.User);
            // Uncomment the next line of code to set the environment variable.
            //  ------------------------------------------------------------------
            // | WARNING!                                                         |
            // |                                                                  |
            // | MAKE SURE TO DELETE YOUR TOKEN AFTER YOU HAVE SET THE VARIABLE   |
            // |                                                                  |
            //  ------------------------------------------------------------------

            //Environment.SetEnvironmentVariable("DiscordSlashCommandsBotToken",
            //    "[YOUR TOKEN GOES HERE    DELETE & COMMENT AFTER USE]",
            //    EnvironmentVariableTarget.User);
        }

        public async Task RunAsync()
        {
            await socketClient.LoginAsync(TokenType.Bot, botToken);
            await socketClient.StartAsync();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            await Task.Delay(-1);
        }

        private async Task InteractionHandler(SocketInteraction arg)
        {
            if(arg.Type == InteractionType.ApplicationCommand)
            {
                await _commands.ExecuteAsync(arg);
            }
        }

        private Task SocketClient_Log(LogMessage arg)
        {
            Console.WriteLine("[Discord] " + arg.ToString());
            return Task.CompletedTask;
        }
    }
}
