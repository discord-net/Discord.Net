using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[Loadable(nameof(Routes.GetUser))]
public partial interface IUserActor :
    IActor<ulong, IUser>,
    IEntityProvider<IDMChannel, IDMChannelModel>
{
    [return: TypeHeuristic(nameof(CreateEntity))]
    async Task<IDMChannel> CreateDMAsync(RequestOptions? options = null, CancellationToken token = default)
    {
        var model = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.CreateDm(new CreateDMChannelParams {RecipientId = Id}),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return CreateEntity(model);
    }
}
