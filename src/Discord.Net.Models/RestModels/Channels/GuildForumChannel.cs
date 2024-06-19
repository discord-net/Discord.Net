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

    int IGuildForumChannelModel.DefaultAutoArchiveDuration => DefaultAutoArchiveDuration;

    int? IGuildForumChannelModel.RatelimitPerUser => RatelimitPerUser;

    int? IGuildForumChannelModel.DefaultThreadRateLimitPerUser => DefaultThreadRatelimitPerUser;

    IEmote? IGuildForumChannelModel.DefaultReactionEmoji => ~DefaultReactionEmoji;

    IEnumerable<IForumTagModel> IGuildForumChannelModel.AvailableTags => AvailableTags | [];
}
