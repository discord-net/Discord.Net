using Discord.Models;

namespace Discord;

public interface ICurrentUserActor :
    IActor<Snowflake, ICurrentUser>,
    IUserActor,
    IModifiable<IModifyCurrentUserParams, ICurrentUser>,
    ILoadable<ICurrentUser>
{
    new ValueTask<ICurrentUser> GetAsync(RequestOptions options = default);

    async ValueTask<IUser> ILoadable<IUser>.GetAsync(RequestOptions options) => await GetAsync(options);
    ValueTask<ICurrentUser> ILoadable<ICurrentUser>.GetAsync(RequestOptions options) => GetAsync(options);
}