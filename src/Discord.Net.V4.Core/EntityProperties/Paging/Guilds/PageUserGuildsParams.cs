using Discord.Models;
using Discord.Rest;

namespace Discord;

public sealed record PageUserGuildsParams(
    int? PageSize = 200,
    int? Total = null,
    EntityOrId<ulong, IPartialGuild>? Before = null,
    EntityOrId<ulong, IPartialGuild>? After = null,
    bool? WithApproximateMemberCounts = null
) : IBetweenPagingParams<ulong>, IPagingParams<PageUserGuildsParams, IEnumerable<IPartialGuildModel>>
{
    public static int MaxPageSize => DiscordConfig.MaxUsersGuildsPerBatch;

    Optional<ulong> IBetweenPagingParams<ulong>.Before => Optional.FromNullable(Before?.Id);

    Optional<ulong> IBetweenPagingParams<ulong>.After => Optional.FromNullable(After?.Id);

    public static IApiOutRoute<IEnumerable<IPartialGuildModel>>? GetRoute(
        PageUserGuildsParams? self,
        IPathable path,
        IEnumerable<IPartialGuildModel>? lastRequest)
    {
        var pageSize = IPagingParams.GetPageSize(self);

        if (self is {Before: not null, After: not null})
        {
            if (lastRequest is null)
            {
                return Routes.GetCurrentUserGuilds(
                    self.Before,
                    limit: pageSize,
                    withCounts: self.WithApproximateMemberCounts
                );
            }

            var min = lastRequest.MinBy(x => x.Id)?.Id;

            if (!min.HasValue || self.After.Value > min.Value)
                return null;

            return Routes.GetCurrentUserGuilds(
                before: min,
                limit: pageSize,
                withCounts: self.WithApproximateMemberCounts
            );
        }

        if (lastRequest is null)
        {
            return Routes.GetCurrentUserGuilds(
                self?.Before,
                self?.After,
                pageSize,
                self?.WithApproximateMemberCounts
            );
        }

        ulong? nextId;

        if (self?.After.HasValue ?? false)
        {
            nextId = lastRequest.MaxBy(x => x.Id)?.Id;

            if (!nextId.HasValue)
                return null;

            return Routes.GetCurrentUserGuilds(
                after: nextId.Value,
                limit: pageSize,
                withCounts: self.WithApproximateMemberCounts
            );
        }

        nextId = lastRequest.MinBy(x => x.Id)?.Id;

        if (!nextId.HasValue)
            return null;

        return Routes.GetCurrentUserGuilds(
            before: nextId,
            limit: pageSize,
            withCounts: self?.WithApproximateMemberCounts
        );
    }
}
