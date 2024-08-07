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

    IEmoteModel? IGuildForumChannelModel.DefaultReactionEmoji => ~DefaultReactionEmoji;

    IEnumerable<ITagModel> IGuildForumChannelModel.AvailableTags => AvailableTags | [];
}
