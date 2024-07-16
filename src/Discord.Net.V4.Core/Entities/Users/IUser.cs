using Discord.Models;
using Discord.Rest;

namespace Discord;

[Refreshable(nameof(Routes.GetUser))]
public partial interface IUser :
    ISnowflakeEntity<IUserModel>,
    IMentionable,
    IUserActor
{
    string? AvatarId { get; }
    ushort Discriminator { get; }
    string Username { get; }
    string? GlobalName { get; }
    bool IsBot { get; }
    UserFlags PublicFlags { get; }

    string IMentionable.Mention => $"<@{Id}>";
}
