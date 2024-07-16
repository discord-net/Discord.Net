using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[ChannelTypeOf(ChannelType.GuildForum)]
public sealed class GuildForumChannelModel : ThreadableChannelModelBase, IGuildForumChannelModel
{
    int IGuildForumChannelModel.DefaultForumLayout => DefaultForumLayout;

    int? IGuildForumChannelModel.DefaultSortOrder => DefaultSortOrder;

    bool IGuildForumChannelModel.IsNsfw => Nsfw;

    string? IGuildForumChannelModel.Topic => Topic;

    int? IGuildForumChannelModel.RatelimitPerUser => RatelimitPerUser;

    IEmoteModel? IGuildForumChannelModel.DefaultReactionEmoji => ~DefaultReactionEmoji;

    IEnumerable<ITagModel> IGuildForumChannelModel.AvailableTags => AvailableTags | [];
}
