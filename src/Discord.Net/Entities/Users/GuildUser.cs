using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.GuildMember;

namespace Discord
{
    public class GuildUser : User, IMentionable
    {
        public Guild Guild { get; }
        public GuildPresence Presence { get; }
        public VoiceState VoiceState { get; }

        /// <inheritdoc />
        public DateTime JoinedAt { get; private set; }
        public GuildPermissions GuildPermissions { get; internal set; }

        public override DiscordClient Discord => Guild.Discord;
        public IEnumerable<TextChannel> TextChannels => Guild.TextChannels.Where(x => GetPermissions(x).ReadMessages);

        internal GuildUser(ulong id, Guild guild, GuildPresence presence, VoiceState voiceState)
            : base(id)
        {
            Guild = guild;
            Presence = presence;
            VoiceState = voiceState;
        }
        internal void Update(Model model)
        {
            base.Update(model.User);
            JoinedAt = model.JoinedAt.Value;
        }

        public ChannelPermissions GetPermissions(GuildChannel channel)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.GetPermissions(this);
        }

        /// <inheritdoc />
        public override Task Update() { throw new NotSupportedException(); } //TODO: Not supported yet
        public Task Kick() => Discord.RestClient.Send(new RemoveGuildMemberRequest(Guild.Id, Id));
        public Task Ban(int pruneDays = 0) => Discord.RestClient.Send(new CreateGuildBanRequest(Guild.Id, Id) { PruneDays = pruneDays });
        public Task Unban() => Discord.RestClient.Send(new RemoveGuildBanRequest(Guild.Id, Id));
    }
}
