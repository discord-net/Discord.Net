using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.GuildMember;

namespace Discord
{
    public class GuildUser : IUser
    {
        private readonly GlobalUser _user;

        public Guild Guild { get; }

        /// <inheritdoc />
        public string CurrentGame { get; private set; }
        /// <inheritdoc />
        public UserStatus Status { get; private set; }
        public VoiceChannel VoiceChannel { get; private set; }

        /// <inheritdoc />
        public DiscordClient Discord => _user.Discord;
        /// <inheritdoc />
        public ulong Id => _user.Id;
        /// <inheritdoc />
        public string Username => _user.Username;
        /// <inheritdoc />
        public ushort Discriminator => _user.Discriminator;
        /// <inheritdoc />
        public bool IsBot => _user.IsBot;
        /// <inheritdoc />
        public string AvatarUrl => _user.AvatarUrl;
        /// <inheritdoc />
        public string Mention => _user.Mention;

        /// <inheritdoc />
        public IReadOnlyList<Role> Roles { get; private set; }

        internal GuildUser(GlobalUser user, Guild guild)
        {
            _user = user;
            Guild = guild;
        }
        internal void Update(Model model)
        {
            JoinedAt = model.JoinedAt.Value;
            Roles = model.Roles.Select(x => Guild.GetRole(x)).ToImmutableArray();
        }

        public bool HasRole(Role role) => false; //TODO: Implement
        
        public Task Kick() => Discord.RestClient.Send(new RemoveGuildMemberRequest(Guild.Id, Id));
        public Task Ban(int pruneDays = 0) => Discord.RestClient.Send(new CreateGuildBanRequest(Guild.Id, Id) { PruneDays = pruneDays });
        public Task Unban() => Discord.RestClient.Send(new RemoveGuildBanRequest(Guild.Id, Id));

        /// <inheritdoc />
        public DateTime JoinedAt { get; private set; }
        public GuildPermissions GuildPermissions { get; internal set; }
        public Task<DMChannel> CreateDMChannel() => _user.CreateDMChannel();

        /// <inheritdoc />
        public Task Update() { throw new NotSupportedException(); } //TODO: Not supported
        
        public ChannelPermissions GetPermissions(GuildChannel channel)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.GetPermissions(this);
        }

        public async Task Modify(Action<ModifyGuildMemberRequest> func)
        {
            if (func != null) throw new NullReferenceException(nameof(func));

            var req = new ModifyGuildMemberRequest(Guild.Id, Id);
            func(req);
            await Discord.RestClient.Send(req).ConfigureAwait(false);
        }
    }
}
