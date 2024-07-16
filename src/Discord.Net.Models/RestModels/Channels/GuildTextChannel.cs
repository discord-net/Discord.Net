using Discord.Converters;

namespace Discord.Models.Json;

[ChannelTypeOf(ChannelType.GuildText)]
public class GuildTextChannelModel : ThreadableChannelModelBase, IGuildTextChannelModel
{
    bool IGuildTextChannelModel.IsNsfw => Nsfw;

    string? IGuildTextChannelModel.Topic => Topic;

    int IGuildTextChannelModel.RatelimitPerUser => RatelimitPerUser;
}
