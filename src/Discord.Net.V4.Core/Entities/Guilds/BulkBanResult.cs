using Discord.Models.Json;
using System.Collections.Immutable;

namespace Discord;

public readonly struct BulkBanResult(ulong[] bannedUsers, ulong[] failedUsers)
    : IConstructable<BulkBanResult, BulkBanResponse>
{
    public readonly IReadOnlyCollection<ulong> BannedUsers = bannedUsers.ToImmutableArray();
    public readonly IReadOnlyCollection<ulong> FailedUsers = failedUsers.ToImmutableArray();

    public static BulkBanResult Construct(IDiscordClient client, BulkBanResponse model) =>
        new(model.BannedUsers, model.FailedUsers);
}
