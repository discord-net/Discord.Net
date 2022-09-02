using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShardedClient.Services
{
    public class InteractionHandlingService
    {
        private readonly InteractionService _service;
        private readonly DiscordShardedClient _client;
        private readonly IServiceProvider _provider;

        public InteractionHandlingService(IServiceProvider services)
        {
            _service = services.GetRequiredService<InteractionService>();
            _client = services.GetRequiredService<DiscordShardedClient>();
            _provider = services;

            _service.Log += LogAsync;
            _client.InteractionCreated += OnInteractionAsync;
            _client.ShardReady += ReadyAsync;
            // For examples on how to handle post execution,
            // see the InteractionFramework samples.
        }

        // Register all modules, and add the commands from these modules to either guild or globally depending on the build state.
        public async Task InitializeAsync()
        {
            await _service.AddModulesAsync(typeof(InteractionHandlingService).Assembly, _provider);
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

        private async Task ReadyAsync(DiscordSocketClient _)
        {
#if DEBUG
            await _service.RegisterCommandsToGuildAsync(1 /* implement */);
#else
            await _service.RegisterCommandsGloballyAsync();
#endif
        }
    }
}
