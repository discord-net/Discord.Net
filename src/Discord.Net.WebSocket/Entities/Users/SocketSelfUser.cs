using Discord.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.ICurrentUserModel;
using UserModel = Discord.IUserModel;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the logged-in WebSocket-based user.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketSelfUser : SocketUser, ISelfUser, ICached<Model>
    {
        /// <inheritdoc />
        public string Email { get; private set; }
        /// <inheritdoc />
        public bool IsVerified { get; private set; }
        /// <inheritdoc />
        public bool IsMfaEnabled { get; private set; }

        /// <inheritdoc />
        public override bool IsBot { get { return GlobalUser.Value.IsBot; } internal set { GlobalUser.Value.IsBot = value; } }
        /// <inheritdoc />
        public override string Username { get { return GlobalUser.Value.Username; } internal set { GlobalUser.Value.Username = value; } }
        /// <inheritdoc />
        public override ushort DiscriminatorValue { get { return GlobalUser.Value.DiscriminatorValue; } internal set { GlobalUser.Value.DiscriminatorValue = value; } }
        /// <inheritdoc />
        public override string AvatarId { get { return GlobalUser.Value.AvatarId; } internal set { GlobalUser.Value.AvatarId = value; } }
        /// <inheritdoc />
        internal override Lazy<SocketPresence> Presence { get { return GlobalUser.Value.Presence; } set { GlobalUser.Value.Presence = value; } }
        /// <inheritdoc />
        public UserProperties Flags { get; internal set; }
        /// <inheritdoc />
        public PremiumType PremiumType { get; internal set; }
        /// <inheritdoc />
        public string Locale { get; internal set; }

        /// <inheritdoc />
        public override bool IsWebhook => false;

        internal SocketSelfUser(DiscordSocketClient discord, ulong userId)
            : base(discord, userId)
        {

        }
        internal static SocketSelfUser Create(DiscordSocketClient discord, Model model)
        {
            var entity = new SocketSelfUser(discord, model.Id);
            entity.Update(model);
            return entity;
        }
        internal override bool Update(UserModel model)
        {
            bool hasGlobalChanges = base.Update(model);

            if (model is not Model currentUserModel)
                throw new ArgumentException($"Got unexpected model type \"{model?.GetType()}\"");

            if(currentUserModel.Email != Email)
            {
                Email = currentUserModel.Email;
                hasGlobalChanges = true;
            }
            if (currentUserModel.IsVerified.HasValue)
            {
                IsVerified = currentUserModel.IsVerified.Value;
                hasGlobalChanges = true;
            }
            if (currentUserModel.IsMfaEnabled.HasValue)
            {
                IsMfaEnabled = currentUserModel.IsMfaEnabled.Value;
                hasGlobalChanges = true;
            }
            if (currentUserModel.Flags != Flags)
            {
                Flags = currentUserModel.Flags;
                hasGlobalChanges = true;
            }
            if (currentUserModel.PremiumType != PremiumType)
            {
                PremiumType = currentUserModel.PremiumType;
                hasGlobalChanges = true;
            }
            if (currentUserModel.Locale != Locale)
            {
                Locale = currentUserModel.Locale;
                hasGlobalChanges = true;
            }
            return hasGlobalChanges;
        }

        /// <inheritdoc />
        public Task ModifyAsync(Action<SelfUserProperties> func, RequestOptions options = null)
            => UserHelper.ModifyAsync(this, Discord, func, options);

        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")}, Self)";
        internal new SocketSelfUser Clone() => MemberwiseClone() as SocketSelfUser;
        public override void Dispose()
        {
            GC.SuppressFinalize(this);
            Discord.StateManager.RemoveReferencedGlobalUser(Id);
        }

        #region Cache
        private struct CacheModel : Model
        {
            public bool? IsVerified { get; set; }

            public string Email { get; set; }

            public bool? IsMfaEnabled { get; set; }

            public UserProperties Flags { get; set; }

            public PremiumType PremiumType { get; set; }

            public string Locale { get; set; }

            public UserProperties PublicFlags { get; set; }

            public string Username { get; set; }

            public string Discriminator { get; set; }

            public bool? IsBot { get; set; }

            public string Avatar { get; set; }

            public ulong Id { get; set; }
        }

        internal new Model ToModel()
            => ToModel<CacheModel>();

        internal new TModel ToModel<TModel>() where TModel : Model, new()
        {
            return new TModel
            {
                Avatar = AvatarId,
                Discriminator = Discriminator,
                Email = Email,
                Flags = Flags,
                Id = Id,
                IsBot = IsBot,
                IsMfaEnabled = IsMfaEnabled,
                IsVerified = IsVerified,
                Locale = Locale,
                PremiumType = this.PremiumType,
                PublicFlags = PublicFlags ?? UserProperties.None,
                Username = Username
            };
        }

        Model ICached<Model>.ToModel() => ToModel();
        TResult ICached<Model>.ToModel<TResult>() => ToModel<TResult>();
        void ICached<Model>.Update(Model model) => Update(model);
        #endregion
    }
}
