using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using Discord.Rest.Pipeline;

namespace Discord;

[
    Loadable<Routes.GetUser>, 
    BackLinkable, 
    Refreshable
]
public partial interface IUserActor :
    IActor<ulong, IUser>
{
    async Task<IDMChannel> CreateDMAsync(RequestOptions? options = null, CancellationToken token = default)
    {
        
        // var model = await Client.RestApiClient.ExecuteRequiredAsync(
        //     Routes.CreateDm(new CreateDMChannelParams {RecipientId = Id}),
        //     options ?? Client.DefaultRequestOptions,
        //     token
        // );

        return await Client.Channels.DM.CreateEntityAsync(model, token);
    }

    [BackLink<IGroupChannelActor>]
    private static async Task AddAsync(
        IGroupChannelActor channel,
        IdOrEntity<ulong, IUserActor> user,
        string accessToken,
        string nickname,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        await channel.Client.RestApiClient.ExecuteAsync(
            Routes.GroupDmAddRecipient(channel.Id, user.Id, new GroupDmAddRecipientParams()
            {
                Nick = nickname,
                AccessToken = accessToken
            }),
            options ?? channel.Client.DefaultRequestOptions,
            token
        );
    }

    [BackLink<IGroupChannelActor>]
    private static async Task RemoveAsync(
        IGroupChannelActor channel,
        IdOrEntity<ulong, IUserActor> user,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        await channel.Client.RestApiClient.ExecuteAsync(
            Routes.GroupDmRemoveRecipient(channel.Id, user.Id),
            options ?? channel.Client.DefaultRequestOptions,
            token
        );
    }

    [LinkExtension]
    private interface WithCurrentExtension
    {
        [LinkMirror(OnlyBackLinks = true)]
        ICurrentUserActor Current { get; }
    }
}
