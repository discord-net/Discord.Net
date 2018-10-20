using System;
using System.Threading.Tasks;
using _03_sharded_client.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace _03_sharded_client
{
    // This is a minimal example of using Discord.Net's Sharded Client
    // The provided DiscordShardedClient class simplifies having multiple
    // DiscordSocketClient instances (or shards) to serve a large number of guilds.
    class Program
    {
        private DiscordShardedClient _client;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();
        public async Task MainAsync()
        {
            // You specify the amount of shards you'd like to have with the
            // DiscordSocketConfig. Generally, it's recommended to 
            // have 1 shard per 1500-2000 guilds your bot is in.
            var config = new DiscordSocketConfig
            {
                TotalShards = 2
            };

            _client = new DiscordShardedClient(config);
            var services = ConfigureServices();

            // The Sharded Client does not have a Ready event.
            // The ShardReady event is used instead, allowing for individual
            // control per shard.
            _client.ShardReady += ReadyAsync;
            _client.Log += LogAsync;

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("token"));
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .BuildServiceProvider();
        }


        private Task ReadyAsync(DiscordSocketClient shard)
        {
            Console.WriteLine($"Shard Number {shard.ShardId} is connected and ready!");
            return Task.CompletedTask;
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }
    }
}
