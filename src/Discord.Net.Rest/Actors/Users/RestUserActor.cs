using Discord.Models;
using Discord.Models.Rest.Actors;
using Discord.Models.Rest.Api;
using Discord.Rest.Actors;
using Discord.Rest.Api;

namespace Discord.Rest;

public class RestUserActor(Snowflake id, DiscordRestClient client) :
    RestActor<Snowflake, RestUser>(id, client), IUserActor
{
    public ValueTask<RestUser> GetAsync(RequestOptions options = default)
    {
        Routes.GetUser
    }

    async ValueTask<IUser> ILoadable<IUser>.GetAsync(RequestOptions options) => await GetAsync(options);
}