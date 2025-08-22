using Discord.Models;
using Discord.Models.Models;

namespace Discord;

public interface IMembersLink :
    IIndexableLink<Snowflake, IMemberActor>,
    IPagedLink<IPageMembersParams, IMember>
{
    ICurrentMemberActor Current { get; }
    
    Task<IMember> AddAsync(IdOrEntity<ulong, IUser> user, RequestOptions options = default);
}