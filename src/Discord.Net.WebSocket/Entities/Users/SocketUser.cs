using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord.Rest;
using Model = Discord.IUserModel;
using PresenceModel = Discord.IPresenceModel;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based user.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public abstract class SocketUser : SocketEntity<ulong>, IUser, ICached<Model>
    {
        /// <inheritdoc />
        public virtual bool IsBot { get; internal set; }
        /// <inheritdoc />
        public virtual string Username { get; internal set; }
        /// <inheritdoc />
        public virtual ushort DiscriminatorValue { get; internal set; }
        /// <inheritdoc />
        public virtual string AvatarId { get; internal set; }
        /// <inheritdoc />
        public virtual bool IsWebhook { get; }
        /// <inheritdoc />
        public UserProperties? PublicFlags { get; private set; }
        internal virtual LazyCached<SocketGlobalUser> GlobalUser { get; set; }
        internal virtual LazyCached<SocketPresence> Presence { get; set; }
        internal bool IsFreed { get; set; }
        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <inheritdoc />
        public string Discriminator => DiscriminatorValue.ToString("D4");
        /// <inheritdoc />
        public string Mention => MentionUtils.MentionUser(Id);
        /// <inheritdoc />
        public UserStatus Status => Presence.Value.Status;
        /// <inheritdoc />
        public IReadOnlyCollection<ClientType> ActiveClients => Presence.Value?.ActiveClients ?? ImmutableHashSet<ClientType>.Empty;
        /// <inheritdoc />
        public IReadOnlyCollection<IActivity> Activities => Presence.Value?.Activities ?? ImmutableList<IActivity>.Empty;
        /// <summary>
        ///     Gets mutual guilds shared with this user.
        /// </summary>
        /// <remarks>
        ///     This property will only include guilds in the same <see cref="DiscordSocketClient"/>.
        /// </remarks>
        public IReadOnlyCollection<SocketGuild> MutualGuilds
            => Discord.Guilds.Where(g => g.GetUser(Id) != null).ToImmutableArray();

        internal SocketUser(DiscordSocketClient discord, ulong id)
            : base(discord, id)
        {
            Presence = new LazyCached<SocketPresence>(id, discord.StateManager.PresenceStore);
            GlobalUser = new LazyCached<SocketGlobalUser>(id, discord.StateManager.UserStore);
        }
        internal virtual bool Update(Model model)
        {
            bool hasChanges = false;
            if (model.Avatar != AvatarId)
            {
                AvatarId = model.Avatar;
                hasChanges = true;
            }
            if (model.Discriminator != null)
            {
                var newVal = ushort.Parse(model.Discriminator, NumberStyles.None, CultureInfo.InvariantCulture);
                if (newVal != DiscriminatorValue)
                {
                    DiscriminatorValue = ushort.Parse(model.Discriminator, NumberStyles.None, CultureInfo.InvariantCulture);
                    hasChanges = true;
                }
            }
            if (model.IsBot.HasValue && model.IsBot.Value != IsBot)
            {
                IsBot = model.IsBot.Value;
                hasChanges = true;
            }
            if (model.Username != Username)
            {
                Username = model.Username;
                hasChanges = true;
            }

            if(model is ICurrentUserModel currentUserModel)
            {
                if (currentUserModel.PublicFlags != PublicFlags)
                {
                    PublicFlags = currentUserModel.PublicFlags;
                    hasChanges = true;
                }
            }

            return hasChanges;
        }

        public abstract void Dispose();

        /// <inheritdoc />
        public async Task<IDMChannel> CreateDMChannelAsync(RequestOptions options = null)
            => await UserHelper.CreateDMChannelAsync(this, Discord, options).ConfigureAwait(false);

        /// <inheritdoc />
        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => CDN.GetUserAvatarUrl(Id, AvatarId, size, format);

        /// <inheritdoc />
        public string GetDefaultAvatarUrl()
            => CDN.GetDefaultUserAvatarUrl(DiscriminatorValue);

        /// <summary>
        ///     Gets the full name of the user (e.g. Example#0001).
        /// </summary>
        /// <returns>
        ///     The full name of the user.
        /// </returns>
        public override string ToString() => Format.UsernameAndDiscriminator(this, Discord.FormatUsersInBidirectionalUnicode);
        private string DebuggerDisplay => $"{Format.UsernameAndDiscriminator(this, Discord.FormatUsersInBidirectionalUnicode)} ({Id}{(IsBot ? ", Bot" : "")})";
        internal SocketUser Clone() => MemberwiseClone() as SocketUser;

        #region Cache 
        internal class CacheModel : Model
        {
            public string Username { get; set; }

            public string Discriminator { get; set; }

            public bool? IsBot { get; set; }

            public string Avatar { get; set; }

            public ulong Id { get; set; }
        }

        internal Model ToModel()
        {
            var model = Discord.StateManager.GetModel<Model, CacheModel>();
            model.Avatar = AvatarId;
            model.Discriminator = Discriminator;
            model.Id = Id;
            model.IsBot = IsBot;
            model.Username = Username;
            return model;
        }

        Model ICached<Model>.ToModel()
            => ToModel();
        void ICached<Model>.Update(Model model) => Update(model);
        bool ICached.IsFreed => IsFreed;

        #endregion
    }
}
