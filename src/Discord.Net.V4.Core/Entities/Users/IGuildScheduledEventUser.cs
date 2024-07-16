using Discord.Models;
using Discord.Rest;

namespace Discord;

[FetchableOfMany(nameof(Routes.GetGuildScheduledEventUsers))]
public partial interface IGuildScheduledEventUser :
    ISnowflakeEntity<IGuildScheduledEventUserModel>,
    IGuildScheduledEventUserActor;
