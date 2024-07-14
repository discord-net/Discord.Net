using Discord.Models;
using Discord.Rest;

namespace Discord;

public interface IUser :
    ISnowflakeEntity,
    IMentionable,
    IUserActor,
    IRefreshable<IUser, ulong, IUserModel>
{
    static IApiOutRoute<IUserModel> IRefreshable<IUser, ulong, IUserModel>.RefreshRoute(IPathable path, ulong id)
        => Routes.GetUser(id);

    string? AvatarId { get; }
    ushort Discriminator { get; }
    string Username { get; }
    string? GlobalName { get; }
    bool IsBot { get; }
    UserFlags PublicFlags { get; }

    string IMentionable.Mention => $"<@{Id}>";
}
