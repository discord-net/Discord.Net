using System.Threading.Tasks;

namespace Discord
{
    public interface IUser : IEntity<ulong>, IMentionable
    {
        bool IsPrivate { get; }

        string Name { get; }
        ushort Discriminator { get; }
        bool IsBot { get; }
        string AvatarId { get; }
        string AvatarUrl { get; }
        string CurrentGame { get; }
        UserStatus Status { get; }

        Task<PrivateChannel> GetPrivateChannel();
    }
}
