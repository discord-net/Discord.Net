using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[ChannelTypeOf(ChannelType.GuildForum)]
public sealed class GuildForumChannel : GuildChannelBase, IGuildForumChannelModel
{
    bool IGuildForumChannelModel.IsNsfw => Nsfw;

    string? IGuildForumChannelModel.Topic => Topic;

    int IGuildForumChannelModel.DefaultAutoArchiveDuration => DefaultAutoArchiveDuration;

    IEnumerable<IForumTagModel> IGuildForumChannelModel.AvailableTags => AvailableTags | [];
}
