using Discord.Models.Models;

namespace Discord.Models;

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