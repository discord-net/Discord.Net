using Discord.Models;
using Discord.Rest;

namespace Discord;

public sealed record PageChannelMessagesParams(
    int? PageSize = null,
    int? Total = null,
    EntityOrId<ulong, IMessage>? Around = null,
    EntityOrId<ulong, IMessage>? Before = null,
    EntityOrId<ulong, IMessage>? After = null
) : IBetweenPagingParams<ulong>, IPagingParams<PageChannelMessagesParams, IEnumerable<IMessageModel>>
{
    public static int MaxPageSize => DiscordConfig.MaxMessagesPerBatch;

    private ulong? _beforeTrack = Before?.Id;
    private ulong? _afterTrack = After?.Id;

    public static IApiOutRoute<IEnumerable<IMessageModel>>? GetRoute(
        PageChannelMessagesParams? self,
        IPathable path,
        IEnumerable<IMessageModel>? lastRequest)
    {
        var pageSize = IPagingParams.GetPageSize(self);

        if (!path.TryGet<ulong, IChannel>(out var channelId))
            return null;

        // if this is the first request
        if (lastRequest is null)
        {
            // and it's an 'around' request
            if (self?.Around is not null)
            {
                return Routes.GetChannelMessages(
                    channelId,
                    self.Around.Value.Id,
                    limit: pageSize
                );
            }

            return self switch
            {
                {Before: not null} => Routes.GetChannelMessages(
                    channelId,
                    beforeId: self.Before.Value.Id,
                    limit: pageSize
                ),
                {After: not null, Before: null} => Routes.GetChannelMessages(
                    channelId,
                    afterId: self.After.Value.Id,
                    limit: pageSize
                ),
                _ => Routes.GetChannelMessages(
                    channelId,
                    limit: pageSize
                )
            };
        }

        if (self?.Around is not null)
        {
            var minId = self._beforeTrack;
            var maxId = self._afterTrack;
            var isBefore = false;
            var isAfter = false;
            var hasAroundId = false;

            var count = 0;

            foreach (var messageModel in lastRequest)
            {
                count++;

                if (minId > messageModel.Id) minId = messageModel.Id;
                if (maxId < messageModel.Id) maxId = messageModel.Id;

                isBefore    |= messageModel.Id < self.Around.Value.Id;
                isAfter     |= messageModel.Id > self.Around.Value.Id;
                hasAroundId |= messageModel.Id == self.Around.Value;
            }

            var expectedCount = isAfter ^ isBefore
                ? pageSize / 2
                : hasAroundId
                    ? (int)Math.Ceiling((pageSize - 1) / 2d)
                    : pageSize / 2;

            switch (isBefore, isAfter)
            {
                case (false, false):
                    return null;
                case (true, false):
                    // if we don't receive the expected count, or the min id is less than the user-supplied min,
                    // we don't want to run a 'before' page again
                    self._beforeTrack = count == expectedCount && !(self.Before >= minId)
                        ? minId
                        : null;

                    if (self._afterTrack is not null)
                    {
                        return Routes.GetChannelMessages(
                            channelId,
                            afterId: self._afterTrack.Value,
                            limit: pageSize
                        );
                    }

                    if (self._beforeTrack is null || !minId.HasValue)
                    {
                        // we don't have a 'before' or 'after' page, this is the end of this pagination
                        return null;
                    }

                    // no more messages to fetch after, so fetch before again
                    return Routes.GetChannelMessages(
                        channelId,
                        beforeId: minId.Value,
                        limit: pageSize
                    );

                case (false, true):
                    // if we don't receive the expected count, or the max id is greater than the user-supplied max,
                    // we don't want to run an 'after' page again
                    self._afterTrack = count == expectedCount && !(self.After <= maxId)
                        ? maxId
                        : null;

                    if (self._beforeTrack is not null)
                    {
                        return Routes.GetChannelMessages(
                            channelId,
                            beforeId: self._beforeTrack.Value,
                            limit: pageSize
                        );
                    }

                    if (self._afterTrack is null || !maxId.HasValue)
                    {
                        // we don't have a 'before' or 'after' page, this is the end of this pagination
                        return null;
                    }

                    // no more messages to fetch before, so fetch after again
                    return Routes.GetChannelMessages(
                        channelId,
                        afterId: maxId.Value,
                        limit: pageSize
                    );
                case (true, true):
                    self._beforeTrack = minId;
                    self._afterTrack = maxId;

                    return (minId, maxId) switch
                    {
                        // order from the api with 'after' is current, new, old, repeating. We reflect that order with
                        // this switch case
                        {maxId: not null} => Routes.GetChannelMessages(
                            channelId,
                            afterId: maxId.Value,
                            limit: pageSize
                        ),
                        {minId: not null} => Routes.GetChannelMessages(
                            channelId,
                            beforeId: minId.Value,
                            limit: pageSize
                        ),
                        _ => (IApiOutRoute<IEnumerable<IMessageModel>>?)null
                    };
            }
        }

        ulong? nextId;

        if (self?.After.HasValue ?? false)
        {
            nextId = lastRequest.MaxBy(x => x.Id)?.Id;

            if (!nextId.HasValue)
                return null;

            return Routes.GetChannelMessages(
                channelId,
                afterId: nextId,
                limit: pageSize
            );
        }

        nextId = lastRequest.MinBy(x => x.Id)?.Id;

        if (!nextId.HasValue)
            return null;

        return Routes.GetChannelMessages(
            channelId,
            beforeId: nextId,
            limit: pageSize
        );
    }

    Optional<ulong> IBetweenPagingParams<ulong>.Before => Optional.FromNullable(Before?.Id);

    Optional<ulong> IBetweenPagingParams<ulong>.After => Optional.FromNullable(After?.Id);
}
