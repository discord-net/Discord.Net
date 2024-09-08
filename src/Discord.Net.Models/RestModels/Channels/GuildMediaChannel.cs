using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), ChannelType.GuildMedia)]
public sealed class GuildMediaChannel : ThreadableChannelBase, IGuildMediaChannelModel
{
    bool IGuildMediaChannelModel.IsNsfw => ~Nsfw;

    string? IGuildMediaChannelModel.Topic => ~Topic;

    int? IGuildMediaChannelModel.RatelimitPerUser => ~RatelimitPerUser;

    DiscordEmojiId? IGuildMediaChannelModel.DefaultReactionEmoji
        => ~DefaultReactionEmoji
            .Map(v => new DiscordEmojiId(v.EmojiName, v.EmojiId, null));

    IEnumerable<ITagModel> IGuildMediaChannelModel.AvailableTags => AvailableTags | [];

    int? IGuildMediaChannelModel.DefaultSortOrder => ~DefaultSortOrder;
}