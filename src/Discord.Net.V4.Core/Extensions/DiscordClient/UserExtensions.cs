using Discord.Models;
using Discord.Rest;

namespace Discord;

public static class DiscordClientUserExtensions
{
    public static async Task<T?> GetUserAsync<T>(this IDiscordClient client, ulong id, RequestOptions? options = null, CancellationToken token = default)
        where T : class, IUser, IConstructable<IUserModel>
    {
        var userModel = await client.RestApiClient.ExecuteAsync(Routes.GetUser(id), options ?? default, token);
        
        return userModel is not null
            ? T.Construct<T>(userModel)
            : null;
    }
}
