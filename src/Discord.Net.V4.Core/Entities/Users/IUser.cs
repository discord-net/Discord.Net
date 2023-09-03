namespace Discord;

public interface IUser : ISnowflakeEntity, IMentionable
{
    string? AvatarId { get; }
    ushort Discriminator { get; }
    string Username { get; }
    string? GlobalName { get; }
    bool IsBot { get; }
    bool IsWebhook { get; }
    UserFlags PublicFlags { get; }

    string IMentionable.Mention => $"<@{Id}>";
}
