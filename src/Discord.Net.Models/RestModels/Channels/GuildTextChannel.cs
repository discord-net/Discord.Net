using Discord.Converters;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), ChannelType.GuildText)]
public class GuildTextChannel : ThreadableChannelBase, IGuildTextChannelModel
{
    bool IGuildTextChannelModel.IsNsfw => ~Nsfw;

    string? IGuildTextChannelModel.Topic => ~Topic;

    int IGuildTextChannelModel.RatelimitPerUser => ~RatelimitPerUser;
}
