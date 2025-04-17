using Discord.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the logged-in WebSocket-based user.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketSelfUser : SocketUser, ISelfUser
    {
        /// <inheritdoc />
        public string Email { get; private set; }
        /// <inheritdoc />
        public bool IsVerified { get; private set; }
        /// <inheritdoc />
        public bool IsMfaEnabled { get; private set; }
        internal override SocketGlobalUser GlobalUser { get; set; }

        /// <summary>
        ///     Gets the hash of the banner.
        /// </summary>
        /// <remarks>
        ///     <see langword="null"/> if the user has no banner set.
        /// </remarks>
        public string BannerId { get; internal set; }

        /// <summary>
        ///     Gets the color of the banner.
        /// </summary>
        /// <remarks>
        ///     <see langword="null"/> if the user has no banner set.
        /// </remarks>
        public Color? BannerColor { get; internal set; }

        /// <inheritdoc />
        public override bool IsBot { get { return GlobalUser.IsBot; } internal set { GlobalUser.IsBot = value; } }
        /// <inheritdoc />
        public override string Username { get { return GlobalUser.Username; } internal set { GlobalUser.Username = value; } }
        /// <inheritdoc />
        public override ushort DiscriminatorValue { get { return GlobalUser.DiscriminatorValue; } internal set { GlobalUser.DiscriminatorValue = value; } }
        /// <inheritdoc />
        public override string AvatarId { get { return GlobalUser.AvatarId; } internal set { GlobalUser.AvatarId = value; } }
        /// <inheritdoc />
        public override string GlobalName { get { return GlobalUser.GlobalName; } internal set { GlobalUser.GlobalName = value; } }
        /// <inheritdoc />
        internal override SocketPresence Presence { get { return GlobalUser.Presence; } set { GlobalUser.Presence = value; } }
        /// <inheritdoc />
        public UserProperties Flags { get; internal set; }
        /// <inheritdoc />
        public PremiumType PremiumType { get; internal set; }
        /// <inheritdoc />
        public string Locale { get; internal set; }

        /// <inheritdoc />
        public override bool IsWebhook => false;

        internal SocketSelfUser(DiscordSocketClient discord, SocketGlobalUser globalUser)
            : base(discord, globalUser.Id)
        {
            GlobalUser = globalUser;
        }
        internal static SocketSelfUser Create(DiscordSocketClient discord, ClientState state, Model model)
        {
            var entity = new SocketSelfUser(discord, discord.GetOrCreateSelfUser(state, model));
            entity.Update(state, model);
            return entity;
        }
        internal override bool Update(ClientState state, Model model)
        {
            bool hasGlobalChanges = base.Update(state, model);
            if (model.Email.IsSpecified)
            {
                Email = model.Email.Value;
                hasGlobalChanges = true;
            }
            if (model.Verified.IsSpecified)
            {
                IsVerified = model.Verified.Value;
                hasGlobalChanges = true;
            }
            if (model.MfaEnabled.IsSpecified)
            {
                IsMfaEnabled = model.MfaEnabled.Value;
                hasGlobalChanges = true;
            }
            if (model.Flags.IsSpecified && model.Flags.Value != Flags)
            {
                Flags = (UserProperties)model.Flags.Value;
                hasGlobalChanges = true;
            }
            if (model.PremiumType.IsSpecified && model.PremiumType.Value != PremiumType)
            {
                PremiumType = model.PremiumType.Value;
                hasGlobalChanges = true;
            }
            if (model.Locale.IsSpecified && model.Locale.Value != Locale)
            {
                Locale = model.Locale.Value;
                hasGlobalChanges = true;
            }

            if (model.BannerColor.IsSpecified && model.BannerColor.Value != BannerColor)
            {
                BannerColor = model.BannerColor.Value;
                hasGlobalChanges = true;
            }

            if (model.Banner.IsSpecified && model.Banner.Value != BannerId)
            {
                BannerId = model.Banner.Value;
                hasGlobalChanges = true;
            }
            return hasGlobalChanges;
        }

        /// <inheritdoc />
        public Task ModifyAsync(Action<SelfUserProperties> func, RequestOptions options = null)
            => UserHelper.ModifyAsync(this, Discord, func, options);

        private string DebuggerDisplay => DiscriminatorValue != 0
            ? $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")}, Self)"
            : $"{Username} ({Id}{(IsBot ? ", Bot" : "")}, Self)";

        internal new SocketSelfUser Clone() => MemberwiseClone() as SocketSelfUser;
    }
}
