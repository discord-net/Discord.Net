using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace ShardedClient.Services
{
    public class CommandHandlingService
    {
        private readonly InteractionService _service;
        private readonly DiscordShardedClient _client;
        private readonly IServiceProvider _provider;

        public CommandHandlingService(IServiceProvider services)
        {
            _service = services.GetRequiredService<InteractionService>();
            _client = services.GetRequiredService<DiscordShardedClient>();
            _provider = services;

            _service.CommandExecuted += CommandExecutedAsync;
            _service.Log += LogAsync;
            _client.InteractionCreated += OnInteractionAsync;
        }

        // Register all modules, and add the commands from these modules to either guild or globally depending on the build state.
        public async Task InitializeAsync()
        {
            await _service.AddModulesAsync(typeof(CommandHandlingService).Assembly, _provider);
#if DEBUG
            await _service.AddCommandsToGuildAsync(/* debug guild ID */);
#else
            await _service.AddCommandsGloballyAsync();
#endif
        }

        private async Task OnInteractionAsync(SocketInteraction interaction)
        {
            _ = Task.Run(async () =>
            {
                var context = new ShardedInteractionContext(_client, interaction);
                await _service.ExecuteCommandAsync(context, _provider);
            });
            await Task.CompletedTask;
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }
    }
}
