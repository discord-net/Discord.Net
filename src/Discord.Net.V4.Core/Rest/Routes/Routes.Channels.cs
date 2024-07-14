using Discord.Models.Json;
using Discord.Utils;
using System.Net;

namespace Discord.Rest;

public static partial class Routes
{
    public static IApiOutRoute<Channel> GetChannel(ulong channelId) =>
        new ApiOutRoute<Channel>(nameof(GetChannel), RequestMethod.Get, $"/channels/{channelId}",
            (ScopeType.Channel, channelId));

    public static IApiInOutRoute<TArgs, Channel> ModifyChannel<TArgs>(ulong id, TArgs body)
        where TArgs : ModifyChannelParams =>
        new ApiInOutRoute<TArgs, Channel>(nameof(ModifyChannel), RequestMethod.Patch, $"/channels/{id}", body,
            ContentType.JsonBody, (ScopeType.Channel, id));

    public static IApiRoute DeleteChannel(ulong channelId) =>
        new ApiRoute(nameof(DeleteChannel), RequestMethod.Delete, $"/channels/{channelId}",
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<Message[]> GetChannelMessages(ulong channelId, ulong? aroundId = default,
        ulong? beforeId = default, ulong? afterId = default, int? limit = default) =>
        new ApiOutRoute<Message[]>(nameof(GetChannelMessages), RequestMethod.Get,
            $"/channels/{channelId}/messages{RouteUtils.GetUrlEncodedQueryParams(("around", aroundId), ("before", beforeId), ("after", afterId), ("limit", limit))}",
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<Message> GetChannelMessage(ulong channelId, ulong messageId) =>
        new ApiOutRoute<Message>(nameof(GetChannelMessage), RequestMethod.Get,
            $"/channels/{channelId}/messages/{messageId}", (ScopeType.Channel, channelId));

    public static IApiInOutRoute<CreateMessageParams, Message>
        CreateMessage(ulong channelId, CreateMessageParams body) =>
        new ApiInOutRoute<CreateMessageParams, Message>(nameof(CreateMessage), RequestMethod.Post,
            $"/channels/{channelId}", body, ContentType.JsonBody, (ScopeType.Channel, channelId));

    // TODO: add support for files
    //public static ApiBodyRoute<Message> CreateMessageWithFiles(ulong channelId)
    //    => new(nameof(CreateMessage), RequestMethod.Post, $"/channels/{channelId}", (ScopeType.Channel, channelId));

    public static IApiOutRoute<Message> CrosspostMessage(ulong channelId, ulong messageId) =>
        new ApiOutRoute<Message>(nameof(CrosspostMessage), RequestMethod.Post,
            $"/channels/{channelId}/messages/{messageId}/crosspost", (ScopeType.Channel, channelId));

    public static IApiRoute CreateReaction(ulong channelId, ulong messageId, string emoji) =>
        new ApiRoute(nameof(CreateReaction), RequestMethod.Put,
            $"/channels/{channelId}/messages/{messageId}/reactions/{WebUtility.UrlEncode(emoji)}/@me",
            (ScopeType.Channel, channelId));

    public static IApiRoute DeleteReaction(ulong channelId, ulong messageId, string emoji) =>
        new ApiRoute(nameof(DeleteReaction), RequestMethod.Delete,
            $"/channels/{channelId}/messages/{messageId}/reactions/{WebUtility.UrlEncode(emoji)}/@me",
            (ScopeType.Channel, channelId));

    public static IApiRoute DeleteUserReaction(ulong channelId, ulong messageId, string emoji, ulong userId) =>
        new ApiRoute(nameof(DeleteUserReaction), RequestMethod.Delete,
            $"/channels/{channelId}/messages/{messageId}/reactions/{WebUtility.UrlEncode(emoji)}/{userId}",
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<Reaction[]> GetReactions(ulong channelId, ulong messageId, string emoji,
        ulong? afterId = default, int? limit = default, ReactionType type = ReactionType.Normal) =>
        new ApiOutRoute<Reaction[]>(nameof(GetReactions), RequestMethod.Get,
            $"/channels/{channelId}/messages/{messageId}/reactions/{WebUtility.UrlEncode(emoji)}{RouteUtils.GetUrlEncodedQueryParams(("after", afterId), ("limit", limit), ("type", type))}",
            (ScopeType.Channel, channelId));

    public static IApiRoute DeleteAllReactions(ulong channelId, ulong messageId) =>
        new ApiRoute(nameof(DeleteAllReactions), RequestMethod.Delete,
            $"/channels/{channelId}/messages/{messageId}/reactions", (ScopeType.Channel, channelId));

    public static IApiRoute DeleteAllReactionsForEmoji(ulong channelId, ulong messageId, string emoji) =>
        new ApiRoute(nameof(DeleteAllReactionsForEmoji), RequestMethod.Delete,
            $"/channels/{channelId}/messages/{messageId}/reactions/{WebUtility.UrlEncode(emoji)}",
            (ScopeType.Channel, channelId));

    public static IApiInOutRoute<ModifyMessageParams, Message> ModifyMessage(ulong channelId, ulong messageId,
        ModifyMessageParams body) =>
        new ApiInOutRoute<ModifyMessageParams, Message>(nameof(ModifyMessage), RequestMethod.Patch,
            $"/channels/{channelId}/messages/{messageId}", body, ContentType.JsonBody, (ScopeType.Channel, channelId));

    public static IApiRoute DeleteMessage(ulong channelId, ulong messageId) =>
        new ApiRoute(nameof(DeleteMessage), RequestMethod.Delete, $"/channels/{channelId}/messages/{messageId}",
            (ScopeType.Channel, channelId));

    public static IApiInRoute<BulkDeleteMessagesParams> BulkDeleteMessages(BulkDeleteMessagesParams body,
        ulong channelId) =>
        new ApiInRoute<BulkDeleteMessagesParams>(nameof(BulkDeleteMessages), RequestMethod.Post,
            $"/channels/{channelId}/messages/bulk-delete", body, ContentType.JsonBody, (ScopeType.Channel, channelId));

    public static IApiInRoute<ModifyChannelPermissionsParams> ModifyChannelPermissions(ulong channelId,
        ulong overwriteId, ModifyChannelPermissionsParams body) =>
        new ApiInRoute<ModifyChannelPermissionsParams>(nameof(ModifyChannelPermissions), RequestMethod.Put,
            $"/channels/{channelId}/permissions/{overwriteId}", body, ContentType.JsonBody,
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<InviteMetadata[]> GetChannelInvites(ulong channelId) =>
        new ApiOutRoute<InviteMetadata[]>(nameof(GetChannelInvites), RequestMethod.Get,
            $"/channels/{channelId}/invites", (ScopeType.Channel, channelId));

    public static IApiInOutRoute<CreateChannelInviteParams, Invite> CreateChannelInvite(ulong channelId,
        CreateChannelInviteParams body) =>
        new ApiInOutRoute<CreateChannelInviteParams, Invite>(nameof(CreateChannelInvite), RequestMethod.Post,
            $"/channels/{channelId}/invites", body, ContentType.JsonBody, (ScopeType.Channel, channelId));

    public static IApiRoute DeleteChannelPermissions(ulong channelId, ulong overwriteId) =>
        new ApiRoute(nameof(DeleteChannelPermissions), RequestMethod.Delete,
            $"/channels/{channelId}/permissions/{overwriteId}", (ScopeType.Channel, channelId));

    public static IApiInOutRoute<FollowAnnouncementChannelParams, Models.Json.FollowedChannel>
        FollowAnnouncementChannel(
            ulong channelId, FollowAnnouncementChannelParams body) =>
        new ApiInOutRoute<FollowAnnouncementChannelParams, Models.Json.FollowedChannel>(
            nameof(FollowAnnouncementChannel), RequestMethod.Post, $"/channels/{channelId}/followers", body,
            ContentType.JsonBody, (ScopeType.Channel, channelId));

    public static IApiRoute TriggerTyping(ulong channelId) =>
        new ApiRoute(nameof(TriggerTyping), RequestMethod.Post, $"/channels/{channelId}/typing",
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<Message[]> GetPinnedMessages(ulong channelId) =>
        new ApiOutRoute<Message[]>(nameof(GetPinnedMessages), RequestMethod.Get, $"/channels/{channelId}/pins",
            (ScopeType.Channel, channelId));

    public static IApiRoute PinMessage(ulong channelId, ulong messageId) =>
        new ApiRoute(nameof(PinMessage), RequestMethod.Put, $"/channels/{channelId}/pins/{messageId}",
            (ScopeType.Channel, channelId));

    public static IApiRoute UnpinMessage(ulong channelId, ulong messageId) =>
        new ApiRoute(nameof(UnpinMessage), RequestMethod.Delete, $"/channels/{channelId}/pins/{messageId}",
            (ScopeType.Channel, channelId));

    public static IApiInRoute<GroupDmAddRecipientParams> GroupDmAddRecipient(ulong channelId, ulong userId,
        GroupDmAddRecipientParams body) =>
        new ApiInRoute<GroupDmAddRecipientParams>(nameof(GroupDmAddRecipient), RequestMethod.Put,
            $"/channels/{channelId}/recipients/{userId}", body, ContentType.JsonBody, (ScopeType.Channel, channelId));

    public static IApiInOutRoute<StartThreadFromMessageParams, Channel> StartThreadFromMessage(ulong channelId,
        ulong messageId, StartThreadFromMessageParams body) =>
        new ApiInOutRoute<StartThreadFromMessageParams, Channel>(nameof(StartThreadFromMessage), RequestMethod.Post,
            $"/channels/{channelId}/messages/{messageId}/threads", body, ContentType.JsonBody,
            (ScopeType.Channel, channelId));

    public static IApiInOutRoute<StartThreadParams, Channel> StartThreadWithoutMessage(ulong channelId,
        StartThreadParams body) =>
        new ApiInOutRoute<StartThreadParams, Channel>(nameof(StartThreadWithoutMessage), RequestMethod.Post,
            $"/channels/{channelId}/threads", body, ContentType.JsonBody, (ScopeType.Channel, channelId));

    //TODO: add support for files
    public static IApiInOutRoute<StartThreadInForumParams, Channel> StartThreadInForum(ulong channelId,
        StartThreadInForumParams body) =>
        new ApiInOutRoute<StartThreadInForumParams, Channel>(nameof(StartThreadInForum), RequestMethod.Post,
            $"/channels/{channelId}/threads", body, ContentType.JsonBody, (ScopeType.Channel, channelId));

    public static IApiRoute JoinThread(ulong channelId) =>
        new ApiRoute(nameof(JoinThread), RequestMethod.Put, $"/channels/{channelId}/thread-members/@me",
            (ScopeType.Channel, channelId));

    public static IApiRoute AddThreadMember(ulong channelId, ulong userId) =>
        new ApiRoute(nameof(AddThreadMember), RequestMethod.Put, $"/channels/{channelId}/thread-members/{userId}",
            (ScopeType.Channel, channelId));

    public static IApiRoute LeaveThread(ulong channelId) =>
        new ApiRoute(nameof(LeaveThread), RequestMethod.Delete, $"/channels/{channelId}/thread-members/@me",
            (ScopeType.Channel, channelId));

    public static IApiRoute RemoveThreadMember(ulong channelId, ulong userId) =>
        new ApiRoute(nameof(RemoveThreadMember), RequestMethod.Delete,
            $"/channels/{channelId}/thread-members/{userId}", (ScopeType.Channel, channelId));

    public static IApiOutRoute<ThreadMember>
        GetThreadMember(ulong channelId, ulong userId, bool? withMember = default) =>
        new ApiOutRoute<ThreadMember>(nameof(GetThreadMember), RequestMethod.Get,
            $"/channels/{channelId}/thread-members/{userId}{RouteUtils.GetUrlEncodedQueryParams(("with_member", withMember))}",
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<ThreadMember[]> ListThreadMembers(ulong channelId, bool? withMember = default,
        ulong? afterId = default, int? limit = default) =>
        new ApiOutRoute<ThreadMember[]>(nameof(ListThreadMembers), RequestMethod.Get,
            $"/channels/{channelId}/thread-members{RouteUtils.GetUrlEncodedQueryParams(("with_member", withMember), ("after", afterId), ("limit", limit))}",
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<ChannelThreads> ListPublicArchivedThreads(ulong channelId,
        DateTimeOffset? beforeId = default, int? limit = default) =>
        new ApiOutRoute<ChannelThreads>(nameof(ListPublicArchivedThreads), RequestMethod.Get,
            $"/channels/{channelId}/threads/archived/public{RouteUtils.GetUrlEncodedQueryParams(("before", beforeId), ("limit", limit))}",
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<ChannelThreads> ListPrivateArchivedThreads(ulong channelId,
        DateTimeOffset? beforeId = default, int? limit = default) =>
        new ApiOutRoute<ChannelThreads>(nameof(ListPrivateArchivedThreads), RequestMethod.Get,
            $"/channels/{channelId}/threads/archived/private{RouteUtils.GetUrlEncodedQueryParams(("before", beforeId), ("limit", limit))}",
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<ChannelThreads> ListJoinedPrivateArchivedThreads(ulong channelId,
        DateTimeOffset? beforeId = default, int? limit = default) =>
        new ApiOutRoute<ChannelThreads>(nameof(ListJoinedPrivateArchivedThreads), RequestMethod.Get,
            $"/channels/{channelId}/users/@me/threads/archived/private{RouteUtils.GetUrlEncodedQueryParams(("before", beforeId), ("limit", limit))}",
            (ScopeType.Channel, channelId));
}
