using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.InviteEvent;

namespace Discord.WebSocket
{
    public class SocketInvite : SocketEntity<string>
    {
        public ISocketMessageChannel Channel { get; private set; }

        public ulong ChannelId { get; private set; }

        public IGuild Guild { get; private set; }

        public ulong GuildId { get; private set; }

        public string Code { get; private set; }

        public SocketUser Inviter { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public int MaxAge { get; private set; }

        public int MaxUses { get; private set; }

        public int Uses { get; set; }

        public bool Temporary { get; private set; }

        internal SocketInvite(DiscordSocketClient discord, Model model)
            : base(discord, model.Code)
        {
        }

        internal static SocketInvite Create(DiscordSocketClient discord, Model model, SocketUser inviter, IGuild guild, ISocketMessageChannel channel)
        {
            var entity = new SocketInvite(discord, model);
            entity.Update(model, inviter, guild, channel);
            return entity;
        }

        internal static SocketInvite Create(DiscordSocketClient discord, Model model, IGuild guild, ISocketMessageChannel channel)
        {
            var entity = new SocketInvite(discord, model);
            entity.Update(model.Code, guild, channel);
            return entity;
        }

        internal void Update(Model model, SocketUser inviter, IGuild guild, ISocketMessageChannel channel)
        {
            Channel = channel;
            ChannelId = model.ChannelId;
            Guild = guild;
            GuildId = model.GuildId;
            Code = model.Code;
            Inviter = inviter;
            CreatedAt = model.CreatedAt.Value;
            MaxAge = model.MaxAge.Value;
            MaxUses = model.MaxUses.Value;
            Uses = model.Uses.Value;
            Temporary = model.Temporary;
        }

        internal void Update(string code, IGuild guild, ISocketMessageChannel channel)
        {
            Code = code;
            Guild = guild;
            GuildId = guild.Id;
            Channel = channel;
            ChannelId = channel.Id;
        }


    }
}
