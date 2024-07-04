using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[ChannelTypeOf(ChannelType.GuildMedia)]
public sealed class GuildMediaChannel : GuildChannelBase, IGuildMediaChannelModel
{
    bool IGuildMediaChannelModel.IsNsfw => Nsfw;

    string? IGuildMediaChannelModel.Topic => Topic;

    int IThreadableChannelModel.DefaultAutoArchiveDuration => DefaultAutoArchiveDuration;

    int? IGuildMediaChannelModel.RatelimitPerUser => RatelimitPerUser;

    int? IThreadableChannelModel.DefaultThreadRateLimitPerUser => DefaultThreadRatelimitPerUser;

    IEmote? IGuildMediaChannelModel.DefaultReactionEmoji => ~DefaultReactionEmoji;

    IEnumerable<ITagModel> IGuildMediaChannelModel.AvailableTags => AvailableTags | [];

    int? IGuildMediaChannelModel.DefaultSortOrder => DefaultSortOrder;
}
