using System.Threading.Tasks;

namespace Discord
{
    //TODO: Should this be linked directly to the Profile when it represents us, instead of maintaining a cache of values?
    public class PrivateUser : IUser
    {
        /// <inheritdoc />
        public EntityState State { get; internal set; }
        /// <inheritdoc />
        public ulong Id { get; }
        /// <summary> Returns the private channel for this user. </summary>
        public PrivateChannel Channel { get; }

        /// <inheritdoc />
        bool IUser.IsPrivate => true;

        /// <inheritdoc />
        public string Name { get; }
        /// <inheritdoc />
        public ushort Discriminator { get; }
        /// <inheritdoc />
        public bool IsBot { get; }
        /// <inheritdoc />
        public string AvatarId { get; }
        /// <inheritdoc />
        public string CurrentGame { get; }
        /// <inheritdoc />
        public UserStatus Status { get; }

        /// <inheritdoc />
        public DiscordClient Discord => Channel.Discord;
        /// <inheritdoc />
        public string AvatarUrl { get; }
        /// <inheritdoc />
        public string Mention { get; }

        /// <inheritdoc />
        Task<PrivateChannel> IUser.GetPrivateChannel() => Task.FromResult(Channel);

        public Task Update() => null;
    }
}
