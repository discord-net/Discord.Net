using Discord.Models;
using Discord.Rest.Api;

namespace Discord.Rest;

public sealed class RestCurrentUserActor(DiscordRestClient client, Snowflake id) :
    RestUserActor(client, id),
    ICurrentUserActor
{
    protected override IRestApiPipeline<RestUser> GetUserPipeline
        => Routes.GetMyUser.Instance
            .AsPipeline()
            .Map<ICurrentUserModel, RestUser>(RestCurrentUser.Create);
    
    public async Task<RestCurrentUser> ModifyAsync(
        IModifyCurrentUserParams properties,
        RequestOptions options = default
    ) => await Routes.UpdateMyUser.Instance
        .AsPipeline(properties)
        .Map(RestCurrentUser.Create)
        .RunAsync(Client, options);

    public new async ValueTask<RestCurrentUser> GetAsync(RequestOptions options = default)
        => (RestCurrentUser) await base.GetAsync(options);

    async ValueTask<ICurrentUser> ICurrentUserActor.GetAsync(RequestOptions options)
        => await GetAsync(options);

    async Task<ICurrentUser> IModifiable<IModifyCurrentUserParams, ICurrentUser>.ModifyAsync(
        IModifyCurrentUserParams properties, RequestOptions options)
        => await ModifyAsync(properties, options);
}