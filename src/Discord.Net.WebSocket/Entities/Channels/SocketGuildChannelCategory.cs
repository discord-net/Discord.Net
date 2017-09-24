using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Audio;
using Discord.Rest;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketGuildChannelCategory : SocketGuildChannel, IGuildChannelCategory
    {
        public override IReadOnlyCollection<SocketGuildUser> Users
            => Guild.Users.Where(x => x.VoiceChannel?.Id == Id).ToImmutableArray();

        internal SocketGuildChannelCategory(DiscordSocketClient discord, ulong id, SocketGuild guild)
            : base(discord, id, guild)
        {
        }
        internal new static SocketGuildChannelCategory Create(SocketGuild guild, ClientState state, Model model)
        {
            var entity = new SocketGuildChannelCategory(guild.Discord, model.Id, guild);
            entity.Update(state, model);
            return entity;
        }
        internal override void Update(ClientState state, Model model)
        {
            base.Update(state, model);
        }

        public override SocketGuildUser GetUser(ulong id)
        {
            var user = Guild.GetUser(id);
            if (user?.VoiceChannel?.Id == Id)
                return user;
            return null;
        }

        private string DebuggerDisplay => $"{Name} ({Id}, Category)";
        internal new SocketGuildChannelCategory Clone() => MemberwiseClone() as SocketGuildChannelCategory;
    }
}
