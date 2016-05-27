using Discord.API.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    //TODO: Unload when there are no more references via DMUser or GuildUser
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class User : IUser
    {
        private string _avatarId;

        /// <inheritdoc />
        public ulong Id { get; }
        internal DiscordClient Discord { get; }

        /// <inheritdoc />
        public ushort Discriminator { get; private set; }
        /// <inheritdoc />
        public bool IsBot { get; private set; }
        /// <inheritdoc />
        public string Username { get; private set; }
        /// <inheritdoc />
        public DMChannel DMChannel { get; internal set; }

        /// <inheritdoc />
        public string AvatarUrl => API.CDN.GetUserAvatarUrl(Id, _avatarId);
        /// <inheritdoc />
        public DateTime CreatedAt => DateTimeUtils.FromSnowflake(Id);
        /// <inheritdoc />
        public string Mention => MentionUtils.Mention(this, false);
        /// <inheritdoc />
        public string NicknameMention => MentionUtils.Mention(this, true);

        internal User(DiscordClient discord, Model model)
        {
            Discord = discord;
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
            var channel = DMChannel;
            if (channel == null)
            {
                var args = new CreateDMChannelParams { RecipientId = Id };
                var model = await Discord.ApiClient.CreateDMChannel(args).ConfigureAwait(false);

                channel = new DMChannel(Discord, this, model);
            }
            return channel;
        }

        public override string ToString() => $"{Username}#{Discriminator}";
        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id})";

        /// <inheritdoc />
        Game? IUser.CurrentGame => null;
        /// <inheritdoc />
        UserStatus IUser.Status => UserStatus.Unknown;

        /// <inheritdoc />
        async Task<IDMChannel> IUser.CreateDMChannel()
            => await CreateDMChannel().ConfigureAwait(false);
    }
}
