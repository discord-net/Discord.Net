using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[ChannelTypeOf(ChannelType.GuildMedia)]
public sealed class GuildMediaChannelModel : ThreadableChannelModelBase, IGuildMediaChannelModel
{
    bool IGuildMediaChannelModel.IsNsfw => Nsfw;

    string? IGuildMediaChannelModel.Topic => Topic;

    int? IGuildMediaChannelModel.RatelimitPerUser => RatelimitPerUser;

    IEmoteModel? IGuildMediaChannelModel.DefaultReactionEmoji => ~DefaultReactionEmoji;

    IEnumerable<ITagModel> IGuildMediaChannelModel.AvailableTags => AvailableTags | [];

    int? IGuildMediaChannelModel.DefaultSortOrder => DefaultSortOrder;
}
