using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableUserActor :
    IUserActor,
    ILoadableEntity<ulong, IUser>;

public interface IUserActor :
    IActor<ulong, IUser>
{
    async Task<IDMChannel> CreateDMAsync(RequestOptions? options = null, CancellationToken token = default)
    {
        var model = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.CreateDm(new CreateDMChannelParams()
            {
                RecipientId = Id
            }),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return Client.CreateEntity(model);
    }
}
