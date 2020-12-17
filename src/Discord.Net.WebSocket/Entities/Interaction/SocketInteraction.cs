using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Gateway.InteractionCreated;

namespace Discord.WebSocket.Entities.Interaction
{
    public class SocketInteraction : SocketEntity<ulong>, IDiscordInteraction
    {
        public SocketGuild Guild
            => Discord.GetGuild(GuildId);
        public SocketTextChannel Channel
            => Guild.GetTextChannel(ChannelId);
        public SocketGuildUser Member
            => Guild.GetUser(MemberId);

        public InteractionType Type { get; private set; }
        public IApplicationCommandInteractionData Data { get; private set; }
        public string Token { get; private set; }
        public int Version { get; private set; }
        public DateTimeOffset CreatedAt { get; }

        public ulong GuildId { get; private set; }
        public ulong ChannelId { get; private set; }
        public ulong MemberId { get; private set; }

       
        internal SocketInteraction(DiscordSocketClient client, ulong id)
            : base(client, id)
        {
        }

        internal static SocketInteraction Create(DiscordSocketClient client, Model model)
        {
            var entitiy = new SocketInteraction(client, model.Id);
            entitiy.Update(model);
            return entitiy;
        }

        internal void Update(Model model)
        {
            this.Data = model.Data.IsSpecified
                ? SocketInteractionData.Create(this.Discord, model.Data.Value)
                : null;

            this.GuildId = model.GuildId;
            this.ChannelId = model.ChannelId;
            this.Token = model.Token;
            this.Version = model.Version;
            this.MemberId = model.Member.User.Id;
            this.Type = model.Type;
        }

    }
}
