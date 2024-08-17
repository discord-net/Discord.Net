using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), ChannelType.GuildForum)]
public sealed class GuildForumChannel : ThreadableChannelBase, IGuildForumChannelModel
{
    int IGuildForumChannelModel.DefaultForumLayout => ~DefaultForumLayout;

    int? IGuildForumChannelModel.DefaultSortOrder => ~DefaultSortOrder;

    bool IGuildForumChannelModel.IsNsfw => ~Nsfw;

    string? IGuildForumChannelModel.Topic => ~Topic;

    int? IGuildForumChannelModel.RatelimitPerUser => ~RatelimitPerUser;

    ulong? IGuildForumChannelModel.DefaultReactionEmojiId => ~DefaultReactionEmoji.Map(v => v.EmojiId);
    string? IGuildForumChannelModel.DefaultReactionEmojiName => ~DefaultReactionEmoji.Map(v => v.EmojiName);

    IEnumerable<ITagModel> IGuildForumChannelModel.AvailableTags => AvailableTags | [];
}
