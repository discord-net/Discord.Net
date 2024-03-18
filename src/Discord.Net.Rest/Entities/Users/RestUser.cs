using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

using EventUserModel = Discord.API.GuildScheduledEventUser;
using Model = Discord.API.User;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based user.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestUser : RestEntity<ulong>, IUser, IUpdateable
    {
        #region RestUser
        /// <inheritdoc />
        public bool IsBot { get; private set; }
        /// <inheritdoc />
        public string Username { get; private set; }
        /// <inheritdoc />
        public ushort DiscriminatorValue { get; private set; }
        /// <inheritdoc />
        public string AvatarId { get; private set; }

        /// <summary>
        ///     Gets the hash of the banner.
        /// </summary>
        /// <remarks>
        ///     <see langword="null"/> if the user has no banner set.
        /// </remarks>
        public string BannerId { get; private set; }

        /// <summary>
        ///     Gets the color of the banner.
        /// </summary>
        /// <remarks>
        ///     <see langword="null"/> if the user has no banner set.
        /// </remarks>
        public Color? BannerColor { get; private set; }

        /// <inheritdoc />
        public Color? AccentColor { get; private set; }
        /// <inheritdoc />
        public UserProperties? PublicFlags { get; private set; }
        /// <inheritdoc />
        public string GlobalName { get; internal set; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <inheritdoc />
        public string Discriminator => DiscriminatorValue.ToString("D4");
        /// <inheritdoc />
        public string Mention => MentionUtils.MentionUser(Id);
        /// <inheritdoc />
        public virtual IActivity Activity => null;
        /// <inheritdoc />
        public virtual UserStatus Status => UserStatus.Offline;
        /// <inheritdoc />
        public virtual IReadOnlyCollection<ClientType> ActiveClients => ImmutableHashSet<ClientType>.Empty;
        /// <inheritdoc />
        public virtual IReadOnlyCollection<IActivity> Activities => ImmutableList<IActivity>.Empty;
        /// <inheritdoc />
        public virtual bool IsWebhook => false;

        /// <inheritdoc />
        public string AvatarDecorationHash { get; private set; }

        /// <inheritdoc />
        public ulong? AvatarDecorationSkuId { get; private set; }


        internal RestUser(BaseDiscordClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static RestUser Create(BaseDiscordClient discord, Model model)
            => Create(discord, null, model, null);
        internal static RestUser Create(BaseDiscordClient discord, IGuild guild, Model model, ulong? webhookId)
        {
            RestUser entity;
            if (webhookId.HasValue)
                entity = new RestWebhookUser(discord, guild, model.Id, webhookId.Value);
            else
                entity = new RestUser(discord, model.Id);
            entity.Update(model);
            return entity;
        }
        internal static RestUser Create(BaseDiscordClient discord, IGuild guild, EventUserModel model)
        {
            if (model.Member.IsSpecified)
            {
                var member = model.Member.Value;
                member.User = model.User;
                return RestGuildUser.Create(discord, guild, member);
            }
            else
                return RestUser.Create(discord, model.User);
        }

        internal virtual void Update(Model model)
        {
            if (model.Avatar.IsSpecified)
                AvatarId = model.Avatar.Value;
            if (model.Banner.IsSpecified)
                BannerId = model.Banner.Value;
            if (model.BannerColor.IsSpecified)
                BannerColor = model.BannerColor.Value;
            if (model.AccentColor.IsSpecified)
                AccentColor = model.AccentColor.Value;
            if (model.Discriminator.IsSpecified)
                DiscriminatorValue = ushort.Parse(model.Discriminator.GetValueOrDefault(null) ?? "0", NumberStyles.None, CultureInfo.InvariantCulture);
            if (model.Bot.IsSpecified)
                IsBot = model.Bot.Value;
            if (model.Username.IsSpecified)
                Username = model.Username.Value;
            if (model.PublicFlags.IsSpecified)
                PublicFlags = model.PublicFlags.Value;
            if (model.GlobalName.IsSpecified)
                GlobalName = model.GlobalName.Value;
            if (model.AvatarDecoration is { IsSpecified: true, Value: not null })
            {
                AvatarDecorationHash = model.AvatarDecoration.Value?.Asset;
                AvatarDecorationSkuId = model.AvatarDecoration.Value?.SkuId;
            }
        }

        /// <inheritdoc />
        public virtual async Task UpdateAsync(RequestOptions options = null)
        {
            var model = await Discord.ApiClient.GetUserAsync(Id, options).ConfigureAwait(false);
            Update(model);
        }

        /// <summary>
        ///     Creates a direct message channel to this user.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a rest DM channel where the user is the recipient.
        /// </returns>
        public Task<RestDMChannel> CreateDMChannelAsync(RequestOptions options = null)
            => UserHelper.CreateDMChannelAsync(this, Discord, options);

        /// <inheritdoc />
        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => CDN.GetUserAvatarUrl(Id, AvatarId, size, format);

        /// <inheritdoc />
        public string GetBannerUrl(ImageFormat format = ImageFormat.Auto, ushort size = 256)
            => CDN.GetUserBannerUrl(Id, BannerId, size, format);

        /// <inheritdoc />
        public string GetDefaultAvatarUrl()
            => DiscriminatorValue != 0
                ? CDN.GetDefaultUserAvatarUrl(DiscriminatorValue)
                : CDN.GetDefaultUserAvatarUrl(Id);

        public virtual string GetDisplayAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => GetAvatarUrl(format, size) ?? GetDefaultAvatarUrl(); 
            
        /// <inheritdoc />
        public string GetAvatarDecorationUrl()
            => AvatarDecorationHash is not null
                ? CDN.GetAvatarDecorationUrl(AvatarDecorationHash)
                : null;

        /// <summary>
        ///     Gets the Username#Discriminator of the user.
        /// </summary>
        /// <returns>
        ///     A string that resolves to Username#Discriminator of the user.
        /// </returns>
        public override string ToString()
            => Format.UsernameAndDiscriminator(this, Discord.FormatUsersInBidirectionalUnicode);

        private string DebuggerDisplay => $"{Format.UsernameAndDiscriminator(this, Discord.FormatUsersInBidirectionalUnicode)} ({Id}{(IsBot ? ", Bot" : "")})";
        #endregion

        #region IUser
        /// <inheritdoc />
        async Task<IDMChannel> IUser.CreateDMChannelAsync(RequestOptions options)
            => await CreateDMChannelAsync(options).ConfigureAwait(false);
        #endregion
    }
}
