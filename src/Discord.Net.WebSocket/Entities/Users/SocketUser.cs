using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord.Rest;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based user.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public abstract class SocketUser : SocketEntity<ulong>, IUser
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
        internal abstract SocketGlobalUser GlobalUser { get; }
        internal abstract SocketPresence Presence { get; set; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <inheritdoc />
        public string Discriminator => DiscriminatorValue.ToString("D4");
        /// <inheritdoc />
        public string Mention => MentionUtils.MentionUser(Id);
        /// <inheritdoc />
        public IActivity Activity => Presence.Activity;
        /// <inheritdoc />
        public UserStatus Status => Presence.Status;
        public IReadOnlyCollection<SocketGuild> MutualGuilds
            => Discord.Guilds.Where(g => g.Users.Any(u => u.Id == Id)).ToImmutableArray();

        internal SocketUser(DiscordSocketClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal virtual bool Update(ClientState state, Model model)
        {
            bool hasChanges = false;
            if (model.Avatar.IsSpecified && model.Avatar.Value != AvatarId)
            {
                AvatarId = model.Avatar.Value;
                hasChanges = true;
            }
            if (model.Discriminator.IsSpecified)
            {
                var newVal = ushort.Parse(model.Discriminator.Value);
                if (newVal != DiscriminatorValue)
                {
                    DiscriminatorValue = ushort.Parse(model.Discriminator.Value);
                    hasChanges = true;
                }
            }
            if (model.Bot.IsSpecified && model.Bot.Value != IsBot)
            {
                IsBot = model.Bot.Value;
                hasChanges = true;
            }
            if (model.Username.IsSpecified && model.Username.Value != Username)
            {
                Username = model.Username.Value;
                hasChanges = true;
            }
            return hasChanges;
        }

        /// <inheritdoc />
        public async Task<IDMChannel> GetOrCreateDMChannelAsync(RequestOptions options = null)
            => GlobalUser.DMChannel ?? await UserHelper.CreateDMChannelAsync(this, Discord, options).ConfigureAwait(false) as IDMChannel;

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
        public override string ToString() => $"{Username}#{Discriminator}";
        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")})";
        internal SocketUser Clone() => MemberwiseClone() as SocketUser;
    }
}
