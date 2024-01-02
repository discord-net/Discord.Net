using Discord.API;
using System.Net;
using Discord.Utils;

namespace Discord.Rest;

public static partial class Routes
{
    public static ApiRoute<Channel> GetChannel(ulong channelId)
        => new(nameof(GetChannel),
            RequestMethod.Get,
            $"/channels/{channelId}",
            (ScopeType.Channel, channelId));

    public static ApiBodyRoute<ModifyChannelParams, Channel> ModifyChannel(ulong channelId, ModifyChannelParams body)
        => new(nameof(ModifyChannel),
            RequestMethod.Patch,
            $"/channels/{channelId}",
            body,
            ContentType.JsonBody,
            (ScopeType.Channel, channelId));

    public static BasicApiRoute DeleteChannel(ulong channelId)
        => new(nameof(DeleteChannel),
            RequestMethod.Delete,
            $"/channels/{channelId}",
            (ScopeType.Channel, channelId));

    public static ApiRoute<Message[]> GetChannelMessages(ulong channelId, ulong? aroundId = default, ulong? beforeId = default, ulong? afterId = default, int? limit = default)
        => new(nameof(GetChannelMessages),
            RequestMethod.Get,
            $"/channels/{channelId}/messages{RouteUtils.GetUrlEncodedQueryParams(("around", aroundId), ("before", beforeId), ("after", afterId), ("limit", limit))}",
            (ScopeType.Channel, channelId));

    public static ApiRoute<Message> GetChannelMessage(ulong channelId, ulong messageId)
        => new(nameof(GetChannelMessage),
            RequestMethod.Get,
            $"/channels/{channelId}/messages/{messageId}",
            (ScopeType.Channel, channelId));

    public static ApiBodyRoute<CreateMessageParams, Message> CreateMessage(ulong channelId, CreateMessageParams body)
        => new(nameof(CreateMessage),
            RequestMethod.Post,
            $"/channels/{channelId}",
            body,
            ContentType.JsonBody,
            (ScopeType.Channel, channelId));

    // TODO: add support for files
    //public static ApiBodyRoute<Message> CreateMessageWithFiles(ulong channelId)
    //    => new(nameof(CreateMessage), RequestMethod.Post, $"/channels/{channelId}", (ScopeType.Channel, channelId));

    public static ApiRoute<Message> CrosspostMessage(ulong channelId, ulong messageId)
        => new(nameof(CrosspostMessage),
            RequestMethod.Post,
            $"/channels/{channelId}/messages/{messageId}/crosspost",
            (ScopeType.Channel, channelId));

    public static BasicApiRoute CreateReaction(ulong channelId, ulong messageId, string emoji)
        => new(nameof(CreateReaction),
            RequestMethod.Put,
            $"/channels/{channelId}/messages/{messageId}/reactions/{WebUtility.UrlEncode(emoji)}/@me",
            (ScopeType.Channel, channelId));

    public static BasicApiRoute DeleteReaction(ulong channelId, ulong messageId, string emoji)
        => new(nameof(DeleteReaction),
            RequestMethod.Delete,
            $"/channels/{channelId}/messages/{messageId}/reactions/{WebUtility.UrlEncode(emoji)}/@me",
            (ScopeType.Channel, channelId));

    public static BasicApiRoute DeleteUserReaction(ulong channelId, ulong messageId, string emoji, ulong userId)
        => new(nameof(DeleteUserReaction),
            RequestMethod.Delete,
            $"/channels/{channelId}/messages/{messageId}/reactions/{WebUtility.UrlEncode(emoji)}/{userId}",
            (ScopeType.Channel, channelId));

    public static ApiRoute<Reaction[]> GetReactions(ulong channelId, ulong messageId, string emoji, ulong? afterId = default, int? limit = default, ReactionType type = ReactionType.Normal)
        => new(nameof(GetReactions),
            RequestMethod.Get,
            $"/channels/{channelId}/messages/{messageId}/reactions/{WebUtility.UrlEncode(emoji)}{RouteUtils.GetUrlEncodedQueryParams(("after", afterId), ("limit", limit), ("type", type))}",
            (ScopeType.Channel, channelId));

    public static BasicApiRoute DeleteAllReactions(ulong channelId, ulong messageId)
        => new(nameof(DeleteAllReactions),
            RequestMethod.Delete,
            $"/channels/{channelId}/messages/{messageId}/reactions",
            (ScopeType.Channel, channelId));

    public static BasicApiRoute DeleteAllReactionsForEmoji(ulong channelId, ulong messageId, string emoji)
        => new(nameof(DeleteAllReactionsForEmoji),
            RequestMethod.Delete,
            $"/channels/{channelId}/messages/{messageId}/reactions/{WebUtility.UrlEncode(emoji)}",
            (ScopeType.Channel, channelId));

    public static ApiBodyRoute<ModifyMessageParams, Message> ModifyMessage(ulong channelId, ulong messageId, ModifyMessageParams body)
        => new(nameof(ModifyMessage),
            RequestMethod.Patch,
            $"/channels/{channelId}/messages/{messageId}",
            body,
            ContentType.JsonBody,
            (ScopeType.Channel, channelId));

    public static BasicApiRoute DeleteMessage(ulong channelId, ulong messageId)
        => new(nameof(DeleteMessage),
            RequestMethod.Delete,
            $"/channels/{channelId}/messages/{messageId}",
            (ScopeType.Channel, channelId));

    public static ApiBodyRoute<BulkDeleteMessagesParams> BulkDeleteMessages(BulkDeleteMessagesParams body, ulong channelId)
        => new(nameof(BulkDeleteMessages),
            RequestMethod.Post,
            $"/channels/{channelId}/messages/bulk-delete",
            body,
            ContentType.JsonBody,
            (ScopeType.Channel, channelId));

    public static ApiBodyRoute<ModifyChannelPermissionsParams> ModifyChannelPermissions(ulong channelId, ulong overwriteId, ModifyChannelPermissionsParams body)
        => new(nameof(ModifyChannelPermissions),
            RequestMethod.Put,
            $"/channels/{channelId}/permissions/{overwriteId}",
            body,
            ContentType.JsonBody,
            (ScopeType.Channel, channelId));

    public static ApiRoute<InviteMetadata[]> GetChannelInvites(ulong channelId)
        => new(nameof(GetChannelInvites),
            RequestMethod.Get,
            $"/channels/{channelId}/invites",
            (ScopeType.Channel, channelId));

    public static ApiBodyRoute<CreateChannelInviteParams, Invite> CreateChannelInvite(ulong channelId, CreateChannelInviteParams body)
        => new(nameof(CreateChannelInvite),
            RequestMethod.Post,
            $"/channels/{channelId}/invites",
            body,
            ContentType.JsonBody,
            (ScopeType.Channel, channelId));

    public static BasicApiRoute DeleteChannelPermissions(ulong channelId, ulong overwriteId)
    => new(nameof(DeleteChannelPermissions),
        RequestMethod.Delete,
        $"/channels/{channelId}/permissions/{overwriteId}",
        (ScopeType.Channel, channelId));

    public static ApiBodyRoute<FollowAnnouncementChannelParams, FollowedChannel> FollowAnnouncementChannel(ulong channelId, FollowAnnouncementChannelParams body)
        => new(nameof(FollowAnnouncementChannel),
            RequestMethod.Post,
            $"/channels/{channelId}/followers",
            body,
            ContentType.JsonBody,
            (ScopeType.Channel, channelId));

    public static BasicApiRoute TriggerTyping(ulong channelId)
        => new(nameof(TriggerTyping),
            RequestMethod.Post,
            $"/channels/{channelId}/typing",
            (ScopeType.Channel, channelId));

    public static ApiRoute<Message[]> GetPinnedMessages(ulong channelId)
        => new(nameof(GetPinnedMessages),
            RequestMethod.Get,
            $"/channels/{channelId}/pins",
            (ScopeType.Channel, channelId));

    public static BasicApiRoute PinMessage(ulong channelId, ulong messageId)
        => new(nameof(PinMessage),
            RequestMethod.Put,
            $"/channels/{channelId}/pins/{messageId}",
            (ScopeType.Channel, channelId));

    public static BasicApiRoute UnpinMessage(ulong channelId, ulong messageId)
        => new(nameof(UnpinMessage),
            RequestMethod.Delete,
            $"/channels/{channelId}/pins/{messageId}",
            (ScopeType.Channel, channelId));

    public static ApiBodyRoute<GroupDmAddRecipientParams> GroupDmAddRecipient(ulong channelId, ulong userId, GroupDmAddRecipientParams body)
        => new(nameof(GroupDmAddRecipient),
            RequestMethod.Put,
            $"/channels/{channelId}/recipients/{userId}",
            body,
            ContentType.JsonBody,
            (ScopeType.Channel, channelId));

    public static ApiBodyRoute<StartThreadFromMessageParams, Channel> StartThreadFromMessage(ulong channelId, ulong messageId, StartThreadFromMessageParams body)
        => new(nameof(StartThreadFromMessage),
            RequestMethod.Post,
            $"/channels/{channelId}/messages/{messageId}/threads",
            body,
            ContentType.JsonBody,
            (ScopeType.Channel, channelId));

    public static ApiBodyRoute<StartThreadParams, Channel> StartThreadWithoutMessage(ulong channelId, StartThreadParams body)
        => new(nameof(StartThreadWithoutMessage),
            RequestMethod.Post,
            $"/channels/{channelId}/threads",
            body,
            ContentType.JsonBody,
            (ScopeType.Channel, channelId));

    //TODO: add support for files
    public static ApiBodyRoute<StartThreadInForumParams, Channel> StartThreadInForum(ulong channelId, StartThreadInForumParams body)
        => new(nameof(StartThreadInForum),
            RequestMethod.Post,
            $"/channels/{channelId}/threads",
            body,
            ContentType.JsonBody,
            (ScopeType.Channel, channelId));

    public static BasicApiRoute JoinThread(ulong channelId)
        => new(nameof(JoinThread),
            RequestMethod.Put,
            $"/channels/{channelId}/thread-members/@me",
            (ScopeType.Channel, channelId));

    public static BasicApiRoute AddThreadMember(ulong channelId, ulong userId)
        => new(nameof(AddThreadMember),
            RequestMethod.Put,
            $"/channels/{channelId}/thread-members/{userId}",
            (ScopeType.Channel, channelId));

    public static BasicApiRoute LeaveThread(ulong channelId)
        => new(nameof(LeaveThread),
            RequestMethod.Delete,
            $"/channels/{channelId}/thread-members/@me",
            (ScopeType.Channel, channelId));

    public static BasicApiRoute RemoveThreadMember(ulong channelId, ulong userId)
        => new(nameof(RemoveThreadMember),
            RequestMethod.Delete,
            $"/channels/{channelId}/thread-members/{userId}",
            (ScopeType.Channel, channelId));

    public static ApiRoute<ThreadMember> GetThreadMember(ulong channelId, ulong userId, bool? withMember = default)
        => new(nameof(GetThreadMember),
            RequestMethod.Get,
            $"/channels/{channelId}/thread-members/{userId}{RouteUtils.GetUrlEncodedQueryParams(("with_member", withMember))}",
            (ScopeType.Channel, channelId));

    public static ApiRoute<ThreadMember[]> ListThreadMembers(ulong channelId, bool? withMember = default, ulong? afterId = default, int? limit = default)
        => new(nameof(ListThreadMembers),
            RequestMethod.Get,
            $"/channels/{channelId}/thread-members{RouteUtils.GetUrlEncodedQueryParams(("with_member", withMember), ("after", afterId), ("limit", limit))}",
            (ScopeType.Channel, channelId));

    public static ApiRoute<ChannelThreads> ListPublicArchivedThreads(ulong channelId, DateTimeOffset? beforeId = default, int? limit = default)
        => new(nameof(ListPublicArchivedThreads),
            RequestMethod.Get,
            $"/channels/{channelId}/threads/archived/public{RouteUtils.GetUrlEncodedQueryParams(("before", beforeId), ("limit", limit))}",
            (ScopeType.Channel, channelId));

    public static ApiRoute<ChannelThreads> ListPrivateArchivedThreads(ulong channelId, DateTimeOffset? beforeId = default, int? limit = default)
        => new(nameof(ListPrivateArchivedThreads),
            RequestMethod.Get,
            $"/channels/{channelId}/threads/archived/private{RouteUtils.GetUrlEncodedQueryParams(("before", beforeId), ("limit", limit))}",
            (ScopeType.Channel, channelId));

    public static ApiRoute<ChannelThreads> ListJoinedPrivateArchivedThreads(ulong channelId, DateTimeOffset? beforeId = default, int? limit = default)
        => new(nameof(ListJoinedPrivateArchivedThreads),
            RequestMethod.Get,
            $"/channels/{channelId}/users/@me/threads/archived/private{RouteUtils.GetUrlEncodedQueryParams(("before", beforeId), ("limit", limit))}",
            (ScopeType.Channel, channelId));

}
