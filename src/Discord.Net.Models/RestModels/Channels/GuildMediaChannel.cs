using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), ChannelType.GuildMedia)]
public sealed class GuildMediaChannel : ThreadableChannelBase, IGuildMediaChannelModel
{
    bool IGuildMediaChannelModel.IsNsfw => ~Nsfw;

    string? IGuildMediaChannelModel.Topic => ~Topic;

    int? IGuildMediaChannelModel.RatelimitPerUser => ~RatelimitPerUser;

    ulong? IGuildMediaChannelModel.DefaultReactionEmojiId => ~DefaultReactionEmoji.Map(v => v.EmojiId);
    string? IGuildMediaChannelModel.DefaultReactionEmojiName => ~DefaultReactionEmoji.Map(v => v.EmojiName);

    IEnumerable<ITagModel> IGuildMediaChannelModel.AvailableTags => AvailableTags | [];

    int? IGuildMediaChannelModel.DefaultSortOrder => ~DefaultSortOrder;
}
