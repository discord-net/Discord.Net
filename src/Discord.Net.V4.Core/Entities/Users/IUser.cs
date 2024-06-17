namespace Discord;

public interface IUser : ISnowflakeEntity, IMentionable, IUserActor
{
    string? AvatarId { get; }
    ushort Discriminator { get; }
    string Username { get; }
    string? GlobalName { get; }
    bool IsBot { get; }
    UserFlags PublicFlags { get; }

    string IMentionable.Mention => $"<@{Id}>";
}
