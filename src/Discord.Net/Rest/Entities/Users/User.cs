using Discord.API.Rest;
using System;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord.Rest
{
    public abstract class User : IUser
    {
        private string _avatarId;

        /// <inheritdoc />
        public ulong Id { get; }
        internal abstract DiscordClient Discord { get; }

        /// <inheritdoc />
        public ushort Discriminator { get; private set; }
        /// <inheritdoc />
        public bool IsBot { get; private set; }
        /// <inheritdoc />
        public string Username { get; private set; }

        /// <inheritdoc />
        public string AvatarUrl => API.CDN.GetUserAvatarUrl(Id, _avatarId);
        /// <inheritdoc />
        public DateTime CreatedAt => DateTimeHelper.FromSnowflake(Id);
        /// <inheritdoc />
        public string Mention => MentionHelper.Mention(this, false);
        /// <inheritdoc />
        public string NicknameMention => MentionHelper.Mention(this, true);
        /// <inheritdoc />
        public bool IsCurrentUser => Id == Discord.CurrentUser.Id;

        internal User(Model model)
        {
            Id = model.Id;

            Update(model);
        }
        internal virtual void Update(Model model)
        {
            _avatarId = model.Avatar;
            Discriminator = model.Discriminator;
            IsBot = model.Bot;
            Username = model.Username;
        }

        public async Task<DMChannel> CreateDMChannel()
        {
            var args = new CreateDMChannelParams
            {
                RecipientId = Id
            };
            var model = await Discord.BaseClient.CreateDMChannel(args).ConfigureAwait(false);

            return new DMChannel(Discord, model);
        }

        public override string ToString() => $"{Username ?? Id.ToString()}";

        /// <inheritdoc />
        string IUser.CurrentGame => null;
        /// <inheritdoc />
        UserStatus IUser.Status => UserStatus.Unknown;

        /// <inheritdoc />
        async Task<IDMChannel> IUser.CreateDMChannel()
            => await CreateDMChannel().ConfigureAwait(false);
    }
}
