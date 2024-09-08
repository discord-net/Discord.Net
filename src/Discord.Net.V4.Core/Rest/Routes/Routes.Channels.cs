using Discord.Models.Json;
using Discord.Utils;
using System.Net;

namespace Discord.Rest;

public static partial class Routes
{
    public static IApiOutRoute<Channel> GetChannel(
        [IdHeuristic<IChannel>] ulong channelId
    ) => new ApiOutRoute<Channel>(
        nameof(GetChannel),
        RequestMethod.Get,
        $"/channels/{channelId}",
        (ScopeType.Channel, channelId)
    );

    public static IApiOutRoute<T> GetChannel<T>([IdHeuristic<IChannel>] ulong channelId)
        where T : Channel
        => new ApiOutRoute<T>(nameof(GetChannel), RequestMethod.Get, $"/channels/{channelId}",
            (ScopeType.Channel, channelId));

    public static IApiInOutRoute<TArgs, Channel> ModifyChannel<TArgs>([IdHeuristic<IChannel>] ulong channelId,
        TArgs body)
        where TArgs : ModifyChannelParams =>
        new ApiInOutRoute<TArgs, Channel>(nameof(ModifyChannel), RequestMethod.Patch, $"/channels/{channelId}",
            body,
            ContentType.JsonBody, (ScopeType.Channel, channelId));

    public static IApiRoute DeleteChannel([IdHeuristic<IChannel>] ulong channelId) =>
        new ApiRoute(nameof(DeleteChannel), RequestMethod.Delete, $"/channels/{channelId}",
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<Message[]> GetChannelMessages(
        [IdHeuristic<IChannel>] ulong channelId,
        ulong? aroundId = default,
        ulong? beforeId = default,
        ulong? afterId = default,
        int? limit = default
    ) => new ApiOutRoute<Message[]>(
        nameof(GetChannelMessages),
        RequestMethod.Get,
        $"/channels/{channelId}/messages{RouteUtils.GetUrlEncodedQueryParams(("around", aroundId), ("before", beforeId), ("after", afterId), ("limit", limit))}",
        (ScopeType.Channel, channelId)
    );

    public static IApiOutRoute<Message> GetChannelMessage([IdHeuristic<IChannel>] ulong channelId,
        [IdHeuristic<IMessage>] ulong messageId) =>
        new ApiOutRoute<Message>(nameof(GetChannelMessage), RequestMethod.Get,
            $"/channels/{channelId}/messages/{messageId}", (ScopeType.Channel, channelId));

    public static IApiInOutRoute<CreateMessageParams, Message>
        CreateMessage([IdHeuristic<IChannel>] ulong channelId, CreateMessageParams body) =>
        new ApiInOutRoute<CreateMessageParams, Message>(nameof(CreateMessage), RequestMethod.Post,
            $"/channels/{channelId}", body, ContentType.JsonBody, (ScopeType.Channel, channelId));

    // TODO: add support for files
    //public static ApiBodyRoute<Message> CreateMessageWithFiles(ulong channelId)
    //    => new(nameof(CreateMessage), RequestMethod.Post, $"/channels/{channelId}", (ScopeType.Channel, channelId));

    public static IApiOutRoute<Message> CrosspostMessage([IdHeuristic<IChannel>] ulong channelId,
        [IdHeuristic<IMessage>] ulong messageId) =>
        new ApiOutRoute<Message>(nameof(CrosspostMessage), RequestMethod.Post,
            $"/channels/{channelId}/messages/{messageId}/crosspost", (ScopeType.Channel, channelId));

    public static IApiRoute CreateReaction(
        [IdHeuristic<IChannel>] ulong channelId,
        [IdHeuristic<IMessage>] ulong messageId,
        [IdHeuristic<IReaction>] DiscordEmojiId emoji
    ) => new ApiRoute(nameof(CreateReaction), RequestMethod.Put,
        $"/channels/{channelId}/messages/{messageId}/reactions/{emoji.ToURLEncoded()}/@me",
        (ScopeType.Channel, channelId));

    public static IApiRoute DeleteOwnReaction(
        [IdHeuristic<IChannel>] ulong channelId,
        [IdHeuristic<IMessage>] ulong messageId,
        [IdHeuristic<IReaction>] DiscordEmojiId emoji
    ) => new ApiRoute(
        nameof(DeleteOwnReaction),
        RequestMethod.Delete,
        $"/channels/{channelId}/messages/{messageId}/reactions/{emoji.ToURLEncoded()}/@me",
        (ScopeType.Channel, channelId)
    );

    public static IApiRoute DeleteUserReaction(
        [IdHeuristic<IChannel>] ulong channelId,
        [IdHeuristic<IMessage>] ulong messageId,
        [IdHeuristic<IReaction>] DiscordEmojiId emoji,
        [IdHeuristic<IUser>] ulong userId
    ) => new ApiRoute(
        nameof(DeleteUserReaction),
        RequestMethod.Delete,
        $"/channels/{channelId}/messages/{messageId}/reactions/{emoji.ToURLEncoded()}/{userId}",
        (ScopeType.Channel, channelId)
    );

    public static IApiOutRoute<User[]> GetReactions(
        [IdHeuristic<IChannel>] ulong channelId,
        [IdHeuristic<IMessage>] ulong messageId,
        [IdHeuristic<IReaction>] DiscordEmojiId emoji,
        ulong? afterId = default,
        int? limit = default,
        ReactionType? type = null
    ) => new ApiOutRoute<User[]>(
        nameof(GetReactions),
        RequestMethod.Get,
        $"/channels/{channelId}/messages/{messageId}/reactions/{emoji.ToURLEncoded()}{RouteUtils.GetUrlEncodedQueryParams(("after", afterId), ("limit", limit), ("type", type))}",
        (ScopeType.Channel, channelId)
    );

    public static IApiRoute DeleteAllReactions([IdHeuristic<IChannel>] ulong channelId,
        [IdHeuristic<IMessage>] ulong messageId) =>
        new ApiRoute(nameof(DeleteAllReactions), RequestMethod.Delete,
            $"/channels/{channelId}/messages/{messageId}/reactions", (ScopeType.Channel, channelId));

    public static IApiRoute DeleteAllReactionsForEmoji(
        [IdHeuristic<IChannel>] ulong channelId,
        [IdHeuristic<IMessage>] ulong messageId,
        [IdHeuristic<IReaction>] DiscordEmojiId emoji
    ) => new ApiRoute(
        nameof(DeleteAllReactionsForEmoji),
        RequestMethod.Delete,
        $"/channels/{channelId}/messages/{messageId}/reactions/{emoji.ToURLEncoded()}",
        (ScopeType.Channel, channelId)
    );

    public static IApiInOutRoute<ModifyMessageParams, Message> ModifyMessage([IdHeuristic<IChannel>] ulong channelId,
        [IdHeuristic<IMessage>] ulong messageId,
        ModifyMessageParams body) =>
        new ApiInOutRoute<ModifyMessageParams, Message>(nameof(ModifyMessage), RequestMethod.Patch,
            $"/channels/{channelId}/messages/{messageId}", body, ContentType.JsonBody, (ScopeType.Channel, channelId));

    public static IApiRoute DeleteMessage([IdHeuristic<IChannel>] ulong channelId,
        [IdHeuristic<IMessage>] ulong messageId) =>
        new ApiRoute(nameof(DeleteMessage), RequestMethod.Delete, $"/channels/{channelId}/messages/{messageId}",
            (ScopeType.Channel, channelId));

    public static IApiInRoute<BulkDeleteMessagesParams> BulkDeleteMessages(BulkDeleteMessagesParams body,
        ulong channelId) =>
        new ApiInRoute<BulkDeleteMessagesParams>(nameof(BulkDeleteMessages), RequestMethod.Post,
            $"/channels/{channelId}/messages/bulk-delete", body, ContentType.JsonBody, (ScopeType.Channel, channelId));

    public static IApiInRoute<ModifyChannelPermissionsParams> ModifyChannelPermissions(
        [IdHeuristic<IChannel>] ulong channelId,
        ulong overwriteId, ModifyChannelPermissionsParams body) =>
        new ApiInRoute<ModifyChannelPermissionsParams>(nameof(ModifyChannelPermissions), RequestMethod.Put,
            $"/channels/{channelId}/permissions/{overwriteId}", body, ContentType.JsonBody,
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<InviteMetadata[]> GetChannelInvites([IdHeuristic<IChannel>] ulong channelId) =>
        new ApiOutRoute<InviteMetadata[]>(nameof(GetChannelInvites), RequestMethod.Get,
            $"/channels/{channelId}/invites", (ScopeType.Channel, channelId));

    public static IApiInOutRoute<CreateChannelInviteParams, Invite> CreateChannelInvite(
        [IdHeuristic<IChannel>] ulong channelId,
        CreateChannelInviteParams body) =>
        new ApiInOutRoute<CreateChannelInviteParams, Invite>(nameof(CreateChannelInvite), RequestMethod.Post,
            $"/channels/{channelId}/invites", body, ContentType.JsonBody, (ScopeType.Channel, channelId));

    public static IApiRoute DeleteChannelPermissions([IdHeuristic<IChannel>] ulong channelId, ulong overwriteId) =>
        new ApiRoute(nameof(DeleteChannelPermissions), RequestMethod.Delete,
            $"/channels/{channelId}/permissions/{overwriteId}", (ScopeType.Channel, channelId));

    public static IApiInOutRoute<FollowAnnouncementChannelParams, Models.Json.FollowedChannel>
        FollowAnnouncementChannel(
            [IdHeuristic<INewsChannel>] ulong channelId, FollowAnnouncementChannelParams body
        )
        => new ApiInOutRoute<FollowAnnouncementChannelParams, Models.Json.FollowedChannel>(
            nameof(FollowAnnouncementChannel),
            RequestMethod.Post,
            $"/channels/{channelId}/followers",
            body,
            ContentType.JsonBody,
            (ScopeType.Channel, channelId)
        );

    public static IApiRoute TriggerTyping([IdHeuristic<IChannel>] ulong channelId) =>
        new ApiRoute(nameof(TriggerTyping), RequestMethod.Post, $"/channels/{channelId}/typing",
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<Message[]> GetPinnedMessages([IdHeuristic<IChannel>] ulong channelId) =>
        new ApiOutRoute<Message[]>(nameof(GetPinnedMessages), RequestMethod.Get, $"/channels/{channelId}/pins",
            (ScopeType.Channel, channelId));

    public static IApiRoute PinMessage([IdHeuristic<IChannel>] ulong channelId,
        [IdHeuristic<IMessage>] ulong messageId) =>
        new ApiRoute(nameof(PinMessage), RequestMethod.Put, $"/channels/{channelId}/pins/{messageId}",
            (ScopeType.Channel, channelId));

    public static IApiRoute UnpinMessage([IdHeuristic<IChannel>] ulong channelId,
        [IdHeuristic<IMessage>] ulong messageId) =>
        new ApiRoute(nameof(UnpinMessage), RequestMethod.Delete, $"/channels/{channelId}/pins/{messageId}",
            (ScopeType.Channel, channelId));

    public static IApiInRoute<GroupDmAddRecipientParams> GroupDmAddRecipient(
        [IdHeuristic<IGroupChannel>] ulong channelId, [IdHeuristic<IUser>] ulong userId,
        GroupDmAddRecipientParams body) =>
        new ApiInRoute<GroupDmAddRecipientParams>(nameof(GroupDmAddRecipient), RequestMethod.Put,
            $"/channels/{channelId}/recipients/{userId}", body, ContentType.JsonBody, (ScopeType.Channel, channelId));

    public static IApiRoute GroupDmRemoveRecipient(
        [IdHeuristic<IGroupChannel>] ulong channelId,
        [IdHeuristic<IUser>] ulong userId
    ) => new ApiRoute(
        nameof(GroupDmAddRecipient),
        RequestMethod.Get,
        $"/channels/{channelId}/recipients/{userId}",
        (ScopeType.Channel, channelId)
    );

    public static IApiInOutRoute<StartThreadFromMessageParams, ThreadChannelBase> StartThreadFromMessage(
        [IdHeuristic<IThreadableChannel>] ulong channelId,
        [IdHeuristic<IMessage>] ulong messageId, StartThreadFromMessageParams body) =>
        new ApiInOutRoute<StartThreadFromMessageParams, ThreadChannelBase>(nameof(StartThreadFromMessage),
            RequestMethod.Post,
            $"/channels/{channelId}/messages/{messageId}/threads", body, ContentType.JsonBody,
            (ScopeType.Channel, channelId));

    public static IApiInOutRoute<StartThreadParams, ThreadChannelBase> StartThreadWithoutMessage(
        [IdHeuristic<IThreadableChannel>] ulong channelId,
        StartThreadParams body) =>
        new ApiInOutRoute<StartThreadParams, ThreadChannelBase>(nameof(StartThreadWithoutMessage), RequestMethod.Post,
            $"/channels/{channelId}/threads", body, ContentType.JsonBody, (ScopeType.Channel, channelId));

    //TODO: add support for files
    public static IApiInOutRoute<StartThreadInForumOrMediaParams, ThreadChannelBase> StartThreadInForum(
        [IdHeuristic<IForumChannel>] ulong channelId,
        StartThreadInForumOrMediaParams body) =>
        new ApiInOutRoute<StartThreadInForumOrMediaParams, ThreadChannelBase>(nameof(StartThreadInForum),
            RequestMethod.Post,
            $"/channels/{channelId}/threads", body, ContentType.JsonBody, (ScopeType.Channel, channelId));

    public static IApiRoute JoinThread([IdHeuristic<IThreadChannel>] ulong channelId) =>
        new ApiRoute(nameof(JoinThread), RequestMethod.Put, $"/channels/{channelId}/thread-members/@me",
            (ScopeType.Channel, channelId));

    public static IApiRoute AddThreadMember([IdHeuristic<IThreadChannel>] ulong channelId, ulong userId) =>
        new ApiRoute(nameof(AddThreadMember), RequestMethod.Put, $"/channels/{channelId}/thread-members/{userId}",
            (ScopeType.Channel, channelId));

    public static IApiRoute LeaveThread([IdHeuristic<IThreadChannel>] ulong channelId) =>
        new ApiRoute(nameof(LeaveThread), RequestMethod.Delete, $"/channels/{channelId}/thread-members/@me",
            (ScopeType.Channel, channelId));

    public static IApiRoute RemoveThreadMember([IdHeuristic<IThreadChannel>] ulong channelId, ulong id) =>
        new ApiRoute(nameof(RemoveThreadMember), RequestMethod.Delete,
            $"/channels/{channelId}/thread-members/{id}", (ScopeType.Channel, channelId));

    public static IApiOutRoute<ThreadMember>
        GetThreadMember([IdHeuristic<IThreadChannel>] ulong channelId, [IdHeuristic<IThreadMember>] ulong userId,
            bool? withMember = default) =>
        new ApiOutRoute<ThreadMember>(nameof(GetThreadMember), RequestMethod.Get,
            $"/channels/{channelId}/thread-members/{userId}{RouteUtils.GetUrlEncodedQueryParams(("with_member", withMember))}",
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<ThreadMember[]> ListThreadMembers(
        [IdHeuristic<IThreadChannel>] ulong channelId
    ) => new ApiOutRoute<ThreadMember[]>(
        nameof(ListThreadMembers),
        RequestMethod.Get,
        $"/channels/{channelId}/thread-members",
        (ScopeType.Channel, channelId)
    );
    
    public static IApiOutRoute<ThreadMember[]> ListThreadMembersPaged(
        [IdHeuristic<IThreadChannel>] ulong channelId,
        ulong? afterId = default,
        int? limit = default
    ) => new ApiOutRoute<ThreadMember[]>(
        nameof(ListThreadMembers),
        RequestMethod.Get,
        $"/channels/{channelId}/thread-members{RouteUtils.GetUrlEncodedQueryParams(("with_member", true), ("after", afterId), ("limit", limit))}",
        (ScopeType.Channel, channelId)
    );

    public static IApiOutRoute<ChannelThreads> ListPublicArchivedThreads(
        [IdHeuristic<IThreadableChannel>] ulong channelId,
        DateTimeOffset? beforeId = default, int? limit = default) =>
        new ApiOutRoute<ChannelThreads>(nameof(ListPublicArchivedThreads), RequestMethod.Get,
            $"/channels/{channelId}/threads/archived/public{RouteUtils.GetUrlEncodedQueryParams(("before", beforeId), ("limit", limit))}",
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<ChannelThreads> ListPrivateArchivedThreads(
        [IdHeuristic<IThreadableChannel>] ulong channelId,
        DateTimeOffset? beforeId = default, int? limit = default) =>
        new ApiOutRoute<ChannelThreads>(nameof(ListPrivateArchivedThreads), RequestMethod.Get,
            $"/channels/{channelId}/threads/archived/private{RouteUtils.GetUrlEncodedQueryParams(("before", beforeId), ("limit", limit))}",
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<ChannelThreads> ListJoinedPrivateArchivedThreads(
        [IdHeuristic<IThreadableChannel>] ulong channelId,
        DateTimeOffset? beforeId = default, int? limit = default) =>
        new ApiOutRoute<ChannelThreads>(nameof(ListJoinedPrivateArchivedThreads), RequestMethod.Get,
            $"/channels/{channelId}/users/@me/threads/archived/private{RouteUtils.GetUrlEncodedQueryParams(("before", beforeId), ("limit", limit))}",
            (ScopeType.Channel, channelId));
}