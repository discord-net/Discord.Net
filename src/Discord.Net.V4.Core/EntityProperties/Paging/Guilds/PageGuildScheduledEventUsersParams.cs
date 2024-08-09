using Discord.Models;
using Discord.Rest;

namespace Discord;

public sealed record PageGuildScheduledEventUsersParams(
    int? PageSize = null,
    int? Total = null,
    bool? WithMembers = null,
    EntityOrId<ulong, IUser>? Before = null,
    EntityOrId<ulong, IUser>? After = null
) : IBetweenPagingParams<ulong>,
    IPagingParams<PageGuildScheduledEventUsersParams, IEnumerable<IGuildScheduledEventUserModel>>
{
    public static int MaxPageSize => DiscordConfig.MaxGuildEventUsersPerBatch;

    Optional<ulong> IBetweenPagingParams<ulong>.Before => Optional.FromNullable(Before?.Id);

    Optional<ulong> IBetweenPagingParams<ulong>.After => Optional.FromNullable(After?.Id);

    public static IApiOutRoute<IEnumerable<IGuildScheduledEventUserModel>>? GetRoute(
        PageGuildScheduledEventUsersParams? self,
        IPathable path,
        IEnumerable<IGuildScheduledEventUserModel>? lastRequest)
    {
        var pageSize = IPagingParams.GetPageSize(self);

        if (
            !path.TryGet<ulong, IGuild>(out var guildId) ||
            !path.TryGet<ulong, IGuildScheduledEvent>(out var eventId)
        ) return null;

        if (self is {Before: not null, After: not null})
        {
            if (lastRequest is null)
            {
                return Routes.GetGuildScheduledEventUsers(
                    guildId,
                    eventId,
                    limit:pageSize,
                    withMembers: self.WithMembers,
                    beforeId:self.Before
                );
            }

            var min = lastRequest.MinBy(x => x.Id)?.Id;

            if (!min.HasValue || self.After.Value > min.Value)
                return null;

            return Routes.GetGuildScheduledEventUsers(
                guildId,
                eventId,
                limit:pageSize,
                withMembers: self.WithMembers,
                beforeId: min.Value
            );
        }

        if (lastRequest is null)
        {
            return Routes.GetGuildScheduledEventUsers(
                guildId,
                eventId,
                pageSize,
                self?.WithMembers,
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

            return Routes.GetGuildScheduledEventUsers(
                guildId,
                eventId,
                pageSize,
                self.WithMembers,
                afterId: nextId.Value
            );
        }

        nextId = lastRequest.MinBy(x => x.Id)?.Id;

        if (!nextId.HasValue)
            return null;

        return Routes.GetGuildScheduledEventUsers(
            guildId,
            eventId,
            pageSize,
            self?.WithMembers,
            beforeId: nextId
        );
    }
}
