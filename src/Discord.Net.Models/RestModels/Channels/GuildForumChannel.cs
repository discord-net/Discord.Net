using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[ChannelTypeOf(ChannelType.GuildForum)]
public sealed class GuildForumChannel : GuildChannelBase, IGuildForumChannelModel
{
    int IGuildForumChannelModel.DefaultForumLayout => DefaultForumLayout;

    int? IGuildForumChannelModel.DefaultSortOrder => DefaultSortOrder;

    bool IGuildForumChannelModel.IsNsfw => Nsfw;

    string? IGuildForumChannelModel.Topic => Topic;

    int IThreadableChannelModel.DefaultAutoArchiveDuration => DefaultAutoArchiveDuration;

    int? IGuildForumChannelModel.RatelimitPerUser => RatelimitPerUser;

    int? IThreadableChannelModel.DefaultThreadRateLimitPerUser => DefaultThreadRatelimitPerUser;

    IEmoteModel? IGuildForumChannelModel.DefaultReactionEmoji => ~DefaultReactionEmoji;

    IEnumerable<ITagModel> IGuildForumChannelModel.AvailableTags => AvailableTags | [];
}
