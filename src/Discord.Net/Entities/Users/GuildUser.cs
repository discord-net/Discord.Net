using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.GuildMember;
using PresenceModel = Discord.API.Presence;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class GuildUser : IGuildUser, ISnowflakeEntity
    {
        internal virtual bool IsAttached => false;
        bool IEntity<ulong>.IsAttached => IsAttached;

        private long? _joinedAtTicks;
        
        public string Nickname { get; private set; }
        public GuildPermissions GuildPermissions { get; private set; }

        public Guild Guild { get; private set; }
        public User User { get; private set; }
        public ImmutableArray<Role> Roles { get; private set; }

        public ulong Id => User.Id;
        public string AvatarUrl => User.AvatarUrl;
        public DateTimeOffset CreatedAt => User.CreatedAt;
        public string Discriminator => User.Discriminator;
        public ushort DiscriminatorValue => User.DiscriminatorValue;
        public bool IsBot => User.IsBot;
        public string Mention => MentionUtils.Mention(this, Nickname != null);
        public string Username => User.Username;

        public virtual UserStatus Status => UserStatus.Unknown;
        public virtual Game Game => null;

        public DiscordRestClient Discord => Guild.Discord;
        public DateTimeOffset? JoinedAt => DateTimeUtils.FromTicks(_joinedAtTicks);

        public GuildUser(Guild guild, User user)
        {
            Guild = guild;
            User = user;
            Roles = ImmutableArray.Create<Role>();
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
            
            //if (model.JoinedAt.IsSpecified)
                _joinedAtTicks = model.JoinedAt.UtcTicks;
            if (model.Nick.IsSpecified)
                Nickname = model.Nick.Value;

            //if (model.Roles.IsSpecified)
                UpdateRoles(model.Roles);
        }
        public virtual void Update(PresenceModel model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;
            
            if (model.Roles.IsSpecified)
                UpdateRoles(model.Roles.Value);
            if (model.Nick.IsSpecified)
                Nickname = model.Nick.Value;
        }
        private void Update(ModifyGuildMemberParams args, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            if (args._roleIds.IsSpecified)
                Roles = args._roleIds.Value.Select(x => Guild.GetRole(x)).Where(x => x != null).ToImmutableArray();
            if (args._nickname.IsSpecified)
                Nickname = args._nickname.Value ?? "";
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
            if (isCurrentUser && args._nickname.IsSpecified)
            {
                var nickArgs = new ModifyCurrentUserNickParams { Nickname = args._nickname.Value ?? "" };
                await Discord.ApiClient.ModifyMyNickAsync(Guild.Id, nickArgs).ConfigureAwait(false);
                args._nickname = Optional.Create<string>(); //Remove
            }

            if (!isCurrentUser || args._deaf.IsSpecified || args._mute.IsSpecified || args._roleIds.IsSpecified)
            {
                await Discord.ApiClient.ModifyGuildMemberAsync(Guild.Id, Id, args).ConfigureAwait(false);
                Update(args, UpdateSource.Rest);
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
        
        public async Task<IDMChannel> CreateDMChannelAsync()
        {
            var args = new CreateDMChannelParams { Recipient = this };
            var model = await Discord.ApiClient.CreateDMChannelAsync(args).ConfigureAwait(false);

            return new DMChannel(Discord, new User(model.Recipients.Value[0]), model);
        }

        IGuild IGuildUser.Guild => Guild;
        IReadOnlyCollection<IRole> IGuildUser.Roles => Roles;
        bool IVoiceState.IsDeafened => false;
        bool IVoiceState.IsMuted => false;
        bool IVoiceState.IsSelfDeafened => false;
        bool IVoiceState.IsSelfMuted => false;
        bool IVoiceState.IsSuppressed => false;
        IVoiceChannel IVoiceState.VoiceChannel => null;
        string IVoiceState.VoiceSessionId => null;
    }
}
