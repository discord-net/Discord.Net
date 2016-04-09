namespace Discord
{
    public interface IUser : IEntity<ulong>
    {
        string Username { get; }
        ushort Discriminator { get; }
        bool IsBot { get; }

        string AvatarUrl { get; }
        string Mention { get; }
        string CurrentGame { get; }
        UserStatus Status { get; }
    }
}
