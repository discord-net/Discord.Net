using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IChannelModel : IEntityModel<Snowflake>
{
    ChannelType Type { get; }
    ChannelFlags Flags { get; }
}