using Discord.Models;
using Discord.Rest;

namespace Discord;

public static partial class DiscordClienExtensions
{
    public static Task<T?> FetchUserAsync<T>(
        this IEntityProvider<T, IUser, IUserModel> provider,
        ulong id, RequestOptions? options = null, CancellationToken token = default)
        where T : class, IUser, IConstructable<IUserModel>
        => provider.ExecuteAndConstructAsync(Routes.GetUser(id), options, token);

    public static Task<T> FetchCurrentUserAsync<T>(this IEntityProvider<T, ISelfUser, IUserModel> provider, RequestOptions? options = null, CancellationToken token = default)
        where T : class, ISelfUser, IConstructable<IUserModel>
        => provider.ExecuteAndConstructAsync(Routes.GetCurrentUser, options, token)!;

    public static Task<T?> ModifyCurrentUserAsync<T>(
        this IEntityProvider<T, ISelfUser, IUserModel> provider,
        PropertiesOrModel<ModifySelfUserProperties, API.ModifyCurrentUserParams> properties,
        RequestOptions? options = null, CancellationToken token = default)
        where T : class, ISelfUser, IModifyable<ModifySelfUserProperties>, IConstructable<IUserModel>
    {
        return provider.ExecuteAndConstructAsync(Routes.ModifyCurrentUser(properties.Model), options, token);
    }
}
