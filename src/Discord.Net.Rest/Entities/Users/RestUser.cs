using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestUser : RestEntity<ulong>, IUser, IUpdateable
    {
        /// <inheritdoc />
        public bool IsBot { get; private set; }
        /// <inheritdoc />
        public string Username { get; private set; }
        /// <inheritdoc />
        public ushort DiscriminatorValue { get; private set; }
        /// <inheritdoc />
        public string AvatarId { get; private set; }

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
        public virtual bool IsWebhook => false;

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
        internal virtual void Update(Model model)
        {
            if (model.Avatar.IsSpecified)
                AvatarId = model.Avatar.Value;
            if (model.Discriminator.IsSpecified)
                DiscriminatorValue = ushort.Parse(model.Discriminator.Value);
            if (model.Bot.IsSpecified)
                IsBot = model.Bot.Value;
            if (model.Username.IsSpecified)
                Username = model.Username.Value;
        }

        /// <inheritdoc />
        public virtual async Task UpdateAsync(RequestOptions options = null)
        {
            var model = await Discord.ApiClient.GetUserAsync(Id, options).ConfigureAwait(false);
            Update(model);
        }

        public Task<RestDMChannel> GetOrCreateDMChannelAsync(RequestOptions options = null)
            => UserHelper.CreateDMChannelAsync(this, Discord, options);

        /// <inheritdoc />
        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => CDN.GetUserAvatarUrl(Id, AvatarId, size, format);

        /// <inheritdoc />
        public string GetDefaultAvatarUrl()
            => CDN.GetDefaultUserAvatarUrl(DiscriminatorValue);

        /// <summary>
        ///     Gets the Username#Descriminator of the user.
        /// </summary>
        /// <returns>
        ///     A string that is the Username#Descriminator of the user.
        /// </returns>
        public override string ToString() => $"{Username}#{Discriminator}";
        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")})";

        //IUser
        /// <inheritdoc />
        async Task<IDMChannel> IUser.GetOrCreateDMChannelAsync(RequestOptions options)
            => await GetOrCreateDMChannelAsync(options).ConfigureAwait(false);
    }
}
