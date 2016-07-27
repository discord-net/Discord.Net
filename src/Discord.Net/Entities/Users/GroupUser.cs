using Discord.API.Rest;
using System;
using System.Threading.Tasks;

namespace Discord
{
    internal class GroupUser : IGroupUser
    {
        internal virtual bool IsAttached => false;
        bool IEntity<ulong>.IsAttached => IsAttached;

        public GroupChannel Channel { get; private set; }
        public User User { get; private set; }

        public ulong Id => User.Id;
        public string AvatarUrl => User.AvatarUrl;
        public DateTimeOffset CreatedAt => User.CreatedAt;
        public string Discriminator => User.Discriminator;
        public ushort DiscriminatorValue => User.DiscriminatorValue;
        public bool IsBot => User.IsBot;
        public string Username => User.Username;
        public string Mention => MentionUtils.Mention(this, false);

        public virtual UserStatus Status => UserStatus.Unknown;
        public virtual Game Game => null;

        public DiscordRestClient Discord => Channel.Discord;

        public GroupUser(GroupChannel channel, User user)
        {
            Channel = channel;
            User = user;
        }

        public async Task KickAsync()
        {
            await Discord.ApiClient.RemoveGroupRecipientAsync(Channel.Id, Id).ConfigureAwait(false);
        }

        public async Task<IDMChannel> CreateDMChannelAsync()
        {
            var args = new CreateDMChannelParams { Recipient = this };
            var model = await Discord.ApiClient.CreateDMChannelAsync(args).ConfigureAwait(false);

            return new DMChannel(Discord, new User(model.Recipients.Value[0]), model);
        }
    }
}
