using Discord.API.Gateway;
using System;

namespace Discord.Gateway.EventProcessors
{
    internal sealed class GuildsProcessor : EventProcessor
    {
        public GuildsProcessor()
            : base(EventNames.GuildCreate, EventNames.GuildUpdate, EventNames.GuildDelete)
        {
        }

        public override async ValueTask ProcessAsync(DiscordGatewayClient client, string name, object? payload, CancellationToken token)
        {
            switch (name)
            {
                case EventNames.GuildCreate:
                    var extendedGuild = PayloadAs<ExtendedGuild>(client, payload)
                        ?? throw new NullReferenceException($"Got null payload for {name}");

                    if (extendedGuild.Unavailable == true)
                    {
                        await ProcessUnavailableGuildAsync(client, extendedGuild);
                        return;
                    }

                    var handle = await client.State.Guilds.CreateAsync(extendedGuild, token);

                    client.QueueEvent<GuildCreatedEvent>(handle.Entity);
                    break;
            }
        }

        private ValueTask ProcessUnavailableGuildAsync(DiscordGatewayClient client, ExtendedGuild guild)
        {
            
        }
    }
}

