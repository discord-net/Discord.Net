using Discord.Converters;

namespace Discord.Models.Json;

[ChannelTypeOf(ChannelType.GuildText)]
public class GuildTextChannel : GuildChannelBase, IGuildTextChannelModel
{
    bool IGuildTextChannelModel.IsNsfw => Nsfw;

    string? IGuildTextChannelModel.Topic => Topic;

    int? IGuildTextChannelModel.RatelimitPerUser => RatelimitPerUser;

    int IGuildTextChannelModel.DefaultArchiveDuration => DefaultAutoArchiveDuration;
}
