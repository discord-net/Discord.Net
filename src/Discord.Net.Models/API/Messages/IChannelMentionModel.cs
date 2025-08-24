using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IChannelMentionModel : IEntityModel<Snowflake>
{
    Snowflake GuildId { get; }
    ChannelType Type { get; }
    string Name { get; }
}