using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), ChannelType.GuildMedia)]
public sealed class GuildMediaChannel : ThreadableChannelBase, IGuildMediaChannelModel
{
    bool IGuildMediaChannelModel.IsNsfw => ~Nsfw;

    string? IGuildMediaChannelModel.Topic => ~Topic;

    int? IGuildMediaChannelModel.RatelimitPerUser => ~RatelimitPerUser;

    IEmoteModel? IGuildMediaChannelModel.DefaultReactionEmoji => ~DefaultReactionEmoji;

    IEnumerable<ITagModel> IGuildMediaChannelModel.AvailableTags => AvailableTags | [];

    int? IGuildMediaChannelModel.DefaultSortOrder => ~DefaultSortOrder;
}
