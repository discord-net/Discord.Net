using Discord.Models;

namespace Discord;

public interface IBansLink :
    IIndexableLink<Snowflake, IBannedUserActor>,
    IPagedLink<IPageBansParams, IBannedUser>,
    ICreatable<ICreateBanParams, IBannedUserActor>
{
    Task<IBulkBanResponseModel> CreateBulkAsync(
        ICreateBanParams parameters,
        RequestOptions options = default
    );
}