using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Entities.Interaction
{
    public class SocketInteraction : SocketEntity<ulong>, IDiscordInteraction
    {
        public ulong Id { get; }

        public InteractionType Type { get; }

        public IApplicationCommandInteractionData Data { get; }

        public ulong GuildId { get; }

        public ulong ChannelId { get; }

        public IGuildUser Member { get; }

        public string Token { get; }

        public int Version { get; }

        public DateTimeOffset CreatedAt { get; }
        public SocketInteraction(DiscordSocketClient client, ulong id)
            : base(client, id)
        {

        }
    }
}
