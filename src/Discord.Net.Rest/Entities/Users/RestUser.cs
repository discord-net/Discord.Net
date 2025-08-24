using Discord.Models;
using Discord.Rest.Api;

namespace Discord.Rest;

public class RestUser :
    RestEntity<Snowflake, IUserModel>,
    IRestEntity<RestUser, Snowflake, IUserModel>,
    IUser
{
    protected virtual RestUserActor Actor { get; }

    protected RestUser(DiscordRestClient client, IUserModel model, RestUserActor? actor = null) : base(client, model)
    {
        Actor = actor ?? client.Users[model.Id];
    }

    public static RestUser Create(DiscordRestClient client, IUserModel model)
        => model switch
        {
            ICurrentUserModel currentUserModel => RestCurrentUser.Create(client, model),
            _ => new RestUser(client, model)
        };

    ValueTask<IUser> ILoadable<IUser>.GetAsync(RequestOptions options) => ValueTask.FromResult<IUser>(this);
}