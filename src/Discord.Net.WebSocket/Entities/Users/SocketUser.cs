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
    public abstract class SocketUser : SocketEntity<ulong>, IUser, ICached<Model>, IDisposable
    {
        /// <inheritdoc />
        public abstract bool IsBot { get; internal set; }
        /// <inheritdoc />
        public abstract string Username { get; internal set; }
        /// <inheritdoc />
        public abstract ushort DiscriminatorValue { get; internal set; }
        /// <inheritdoc />
        public abstract string AvatarId { get; internal set; }
        /// <inheritdoc />
        public abstract bool IsWebhook { get; }
        /// <inheritdoc />
        public UserProperties? PublicFlags { get; private set; }
        internal abstract SocketGlobalUser GlobalUser { get; set; }
        internal virtual Lazy<SocketPresence> Presence { get; set; }

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
        }
        internal virtual bool Update(ClientStateManager state, Model model)
        {
            Presence ??= new Lazy<SocketPresence>(() => state.GetPresence(Id), System.Threading.LazyThreadSafetyMode.PublicationOnly);
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
        ~SocketUser() => GlobalUser?.Dispose();
        public void Dispose() => GlobalUser?.Dispose();
        private string DebuggerDisplay => $"{Format.UsernameAndDiscriminator(this, Discord.FormatUsersInBidirectionalUnicode)} ({Id}{(IsBot ? ", Bot" : "")})";
        internal SocketUser Clone() => MemberwiseClone() as SocketUser;

        #region Cache 
        private struct CacheModel : Model
        {
            public string Username { get; set; }

            public string Discriminator { get; set; }

            public bool? IsBot { get; set; }

            public string Avatar { get; set; }

            public ulong Id { get; set; }
        }

        Model ICached<Model>.ToModel()
            => ToModel();

        internal Model ToModel()
        {
            return new CacheModel
            {
                Avatar = AvatarId,
                Discriminator = Discriminator,
                Id = Id,
                IsBot = IsBot,
                Username = Username
            };
        }

        #endregion
    }
}
