using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.GuildMember;

namespace Discord.WebSocket
{
    public class GuildUser : IGuildUser
    {
        private ImmutableArray<Role> _roles;

        public Guild Guild { get; }
        public User GlobalUser { get; }

        /// <inheritdoc />
        public bool IsDeaf { get; private set; }
        /// <inheritdoc />
        public bool IsMute { get; private set; }
        /// <inheritdoc />
        public DateTime JoinedAt { get; private set; }
        /// <inheritdoc />
        public string Nickname { get; private set; }
        /// <inheritdoc />
        public UserStatus Status { get; private set; }
        /// <inheritdoc />
        public Game? CurrentGame { get; private set; }
        /// <inheritdoc />
        public VoiceChannel VoiceChannel { get; private set; }
        /// <inheritdoc />
        public GuildPermissions GuildPermissions { get; private set; }

        /// <inheritdoc />
        public IReadOnlyList<Role> Roles => _roles;
        /// <inheritdoc />
        public string AvatarUrl => GlobalUser.AvatarUrl;
        /// <inheritdoc />
        public ushort Discriminator => GlobalUser.Discriminator;
        /// <inheritdoc />
        public bool IsBot => GlobalUser.IsBot;
        /// <inheritdoc />
        public string Username => GlobalUser.Username;
        /// <inheritdoc />
        public DateTime CreatedAt => GlobalUser.CreatedAt;
        /// <inheritdoc />
        public ulong Id => GlobalUser.Id;
        /// <inheritdoc />
        public string Mention => GlobalUser.Mention;
        internal DiscordClient Discord => Guild.Discord;

        internal GuildUser(User globalUser, Guild guild, Model model)
        {
            GlobalUser = globalUser;
            Guild = guild;

            globalUser.Update(model.User);
            Update(model);
        }
        internal void Update(Model model)
        {
            IsDeaf = model.Deaf;
            IsMute = model.Mute;
            JoinedAt = model.JoinedAt.Value;
            Nickname = model.Nick;

            var roles = ImmutableArray.CreateBuilder<Role>(model.Roles.Length + 1);
            roles.Add(Guild.EveryoneRole);
            for (int i = 0; i < model.Roles.Length; i++)
                roles.Add(Guild.GetRole(model.Roles[i]));
            _roles = roles.ToImmutable();

            UpdateGuildPermissions();
        }
        internal void UpdateGuildPermissions()
        {
            GuildPermissions = new GuildPermissions(Permissions.ResolveGuild(this));
        }

        public async Task Modify(Action<ModifyGuildMemberParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildMemberParams();
            func(args);

            bool isCurrentUser = Discord.CurrentUser.Id == Id;
            if (isCurrentUser && args.Nickname.IsSpecified)
            {
                var nickArgs = new ModifyCurrentUserNickParams { Nickname = args.Nickname.Value };
                await Discord.ApiClient.ModifyCurrentUserNick(Guild.Id, nickArgs).ConfigureAwait(false);
                args.Nickname = new API.Optional<string>(); //Remove
            }

            if (!isCurrentUser || args.Deaf.IsSpecified || args.Mute.IsSpecified || args.Roles.IsSpecified)
            {
                await Discord.ApiClient.ModifyGuildMember(Guild.Id, Id, args).ConfigureAwait(false);
                if (args.Deaf.IsSpecified)
                    IsDeaf = args.Deaf.Value;
                if (args.Mute.IsSpecified)
                    IsMute = args.Mute.Value;
                if (args.Nickname.IsSpecified)
                    Nickname = args.Nickname.Value;
                if (args.Roles.IsSpecified)
                    _roles = args.Roles.Value.Select(x => Guild.GetRole(x)).Where(x => x != null).ToImmutableArray();
            }
        }

        public async Task Kick()
        {
            await Discord.ApiClient.RemoveGuildMember(Guild.Id, Id).ConfigureAwait(false);
        }

        public GuildPermissions GetGuildPermissions()
        {
            return new GuildPermissions(Permissions.ResolveGuild(this));
        }
        public ChannelPermissions GetPermissions(IGuildChannel channel)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return new ChannelPermissions(Permissions.ResolveChannel(this, channel, GuildPermissions.RawValue));
        }

        public async Task<DMChannel> CreateDMChannel()
        {
            return await GlobalUser.CreateDMChannel().ConfigureAwait(false);
        }


        IGuild IGuildUser.Guild => Guild;
        IReadOnlyList<IRole> IGuildUser.Roles => Roles;
        IVoiceChannel IGuildUser.VoiceChannel => VoiceChannel;

        ChannelPermissions IGuildUser.GetPermissions(IGuildChannel channel)
            => GetPermissions(channel);
        async Task<IDMChannel> IUser.CreateDMChannel()
            => await CreateDMChannel().ConfigureAwait(false);
        Task IUpdateable.Update() 
            => Task.CompletedTask;

    }
}
