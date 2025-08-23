using Discord.Models;
using Discord.Rest.Actors;
using Discord.Rest.Api;

namespace Discord.Rest;

public class RestUserActor(DiscordRestClient client, Snowflake id) :
    RestActor<Snowflake, RestUser>(client, id),
    IUserActor
{
    protected virtual IRestApiPipeline<RestUser> GetUserPipeline
        => new Routes.GetUser(Id)
            .AsPipeline()
            .Map(RestUser.Create);

    public async ValueTask<RestUser> GetAsync(RequestOptions options = default)
        => await GetUserPipeline.RunAsync(Client, options);

    async ValueTask<IUser> ILoadable<IUser>.GetAsync(RequestOptions options)
        => await GetAsync(options);
}