using Discord.Models;
using Discord.Rest;

namespace Discord;

[PagedFetchableOfMany<PageGuildScheduledEventUsersParams>(nameof(Routes.GetGuildScheduledEventUsers))]
public partial interface IGuildScheduledEventUser :
    ISnowflakeEntity<IGuildScheduledEventUserModel>,
    IGuildScheduledEventUserActor;
