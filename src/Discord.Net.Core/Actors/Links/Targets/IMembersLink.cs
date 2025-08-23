using Discord.Models;

namespace Discord;

public interface IMembersLink :
    IIndexableLink<Snowflake, IMemberActor>,
    IPagedLink<IPageMembersParams, IMember>
{
    ICurrentMemberActor Current { get; }
    
    Task<IMember> AddAsync(IdOrEntity<Snowflake, IUser> user, RequestOptions options = default);
}