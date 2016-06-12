using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.GuildMember;
using PresenceModel = Discord.API.Presence;
using VoiceStateModel = Discord.API.VoiceState;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class GuildUser : IGuildUser, ISnowflakeEntity
    {
        public bool IsDeaf { get; private set; }
        public bool IsMute { get; private set; }
        public DateTime? JoinedAt { get; private set; }
        public string Nickname { get; private set; }
        public GuildPermissions GuildPermissions { get; private set; }

        public Guild Guild { get; private set; }
        public User User { get; private set; }
        public ImmutableArray<Role> Roles { get; private set; }

        public ulong Id => User.Id;
        public string AvatarUrl => User.AvatarUrl;
        public DateTime CreatedAt => User.CreatedAt;
        public string Discriminator => User.Discriminator;
        public bool IsAttached => User.IsAttached;
        public bool IsBot => User.IsBot;
        public string Mention => User.Mention;
        public string Username => User.Username;
        public virtual UserStatus Status => User.Status;
        public virtual Game? Game => User.Game;

        public DiscordClient Discord => Guild.Discord;

        private GuildUser(Guild guild, User user)
        {
            Guild = guild;
            User = user;
        }
        public GuildUser(Guild guild, User user, Model model)
            : this(guild, user)
        {
            Update(model, UpdateSource.Creation);
        }
        public GuildUser(Guild guild, User user, PresenceModel model)
            : this(guild, user)
        {
            Update(model, UpdateSource.Creation);
        }
        public void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            //if (model.Deaf.IsSpecified)
                IsDeaf = model.Deaf;
            //if (model.Mute.IsSpecified)
                IsMute = model.Mute;
            //if (model.JoinedAt.IsSpecified)
                JoinedAt = model.JoinedAt;
            if (model.Nick.IsSpecified)
                Nickname = model.Nick.Value;

            //if (model.Roles.IsSpecified)
                UpdateRoles(model.Roles);
        }
        public void Update(PresenceModel model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;
            
            if (model.Roles.IsSpecified)
                UpdateRoles(model.Roles.Value);
        }
        public void Update(VoiceStateModel model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            IsDeaf = model.Deaf;
            IsMute = model.Mute;
        }
        private void UpdateRoles(ulong[] roleIds)
        {
            var roles = ImmutableArray.CreateBuilder<Role>(roleIds.Length + 1);
            roles.Add(Guild.EveryoneRole);
            for (int i = 0; i < roleIds.Length; i++)
            {
                var role = Guild.GetRole(roleIds[i]);
                if (role != null)
                    roles.Add(role);
            }
            Roles = roles.ToImmutable();
            GuildPermissions = new GuildPermissions(Permissions.ResolveGuild(this));
        }

        public async Task UpdateAsync()
        {
            if (IsAttached) throw new NotSupportedException();

            var model = await Discord.ApiClient.GetGuildMemberAsync(Guild.Id, Id).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        public async Task ModifyAsync(Action<ModifyGuildMemberParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildMemberParams();
            func(args);

            bool isCurrentUser = (await Discord.GetCurrentUserAsync().ConfigureAwait(false)).Id == Id;
            if (isCurrentUser && args.Nickname.IsSpecified)
            {
                var nickArgs = new ModifyCurrentUserNickParams { Nickname = args.Nickname.Value ?? "" };
                await Discord.ApiClient.ModifyMyNickAsync(Guild.Id, nickArgs).ConfigureAwait(false);
                args.Nickname = new Optional<string>(); //Remove
            }

            if (!isCurrentUser || args.Deaf.IsSpecified || args.Mute.IsSpecified || args.Roles.IsSpecified)
            {
                await Discord.ApiClient.ModifyGuildMemberAsync(Guild.Id, Id, args).ConfigureAwait(false);
                if (args.Deaf.IsSpecified)
                    IsDeaf = args.Deaf.Value;
                if (args.Mute.IsSpecified)
                    IsMute = args.Mute.Value;
                if (args.Nickname.IsSpecified)
                    Nickname = args.Nickname.Value ?? "";
                if (args.Roles.IsSpecified)
                    Roles = args.Roles.Value.Select(x => Guild.GetRole(x)).Where(x => x != null).ToImmutableArray();
            }
        }
        public async Task KickAsync()
        {
            await Discord.ApiClient.RemoveGuildMemberAsync(Guild.Id, Id).ConfigureAwait(false);
        }

        public override string ToString() => $"{Username}#{Discriminator}";
        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id})";

        public ChannelPermissions GetPermissions(IGuildChannel channel)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return new ChannelPermissions(Permissions.ResolveChannel(this, channel, GuildPermissions.RawValue));
        }

        public Task<IDMChannel> CreateDMChannelAsync() => User.CreateDMChannelAsync();

        IGuild IGuildUser.Guild => Guild;
        IReadOnlyCollection<IRole> IGuildUser.Roles => Roles;
        bool IVoiceState.IsSelfDeafened => false;
        bool IVoiceState.IsSelfMuted => false;
        bool IVoiceState.IsSuppressed => false;
        IVoiceChannel IVoiceState.VoiceChannel => null;
        string IVoiceState.VoiceSessionId => null;
    }
}
