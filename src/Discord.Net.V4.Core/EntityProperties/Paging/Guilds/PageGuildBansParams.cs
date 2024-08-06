using Discord.Models;
using Discord.Rest;

namespace Discord;

public record PageGuildBansParams(
    int? PageSize = null,
    int? Total = null,
    EntityOrId<ulong, IUser>? Before = null,
    EntityOrId<ulong, IUser>? After = null
) : IBetweenPagingParams<ulong>, IPagingParams<PageGuildBansParams, IEnumerable<IBanModel>>
{
    public static int MaxPageSize => DiscordConfig.MaxBansPerBatch;

    Optional<ulong> IBetweenPagingParams<ulong>.Before => Optional.FromNullable(Before?.Id);

    Optional<ulong> IBetweenPagingParams<ulong>.After => Optional.FromNullable(After?.Id);

    public static IApiOutRoute<IEnumerable<IBanModel>>? GetRoute(
        PageGuildBansParams? self,
        IPathable path,
        IEnumerable<IBanModel>? lastRequest)
    {
        var pageSize = IPagingParams.GetPageSize(self);

        if (!path.TryGet<ulong, IGuild>(out var guildId))
            return null;

        if (self is {Before: not null, After: not null})
        {
            if (lastRequest is null)
            {
                return Routes.GetGuildBans(
                    guildId,
                    pageSize,
                    self.Before
                );
            }

            var min = lastRequest.MinBy(x => x.Id)?.Id;

            if (!min.HasValue || self.After.Value > min.Value)
                return null;

            return Routes.GetGuildBans(
                guildId,
                pageSize,
                min.Value
            );
        }

        if (lastRequest is null)
        {
            return Routes.GetGuildBans(
                guildId,
                pageSize,
                self?.Before,
                self?.After
            );
        }

        ulong? nextId;

        if (self?.After.HasValue ?? false)
        {
            nextId = lastRequest.MaxBy(x => x.Id)?.Id;

            if (!nextId.HasValue)
                return null;

            return Routes.GetGuildBans(
                guildId,
                pageSize,
                after: nextId.Value
            );
        }

        nextId = lastRequest.MinBy(x => x.Id)?.Id;

        if (!nextId.HasValue)
            return null;

        return Routes.GetGuildBans(
            guildId,
            pageSize,
            before: nextId
        );
    }
}
