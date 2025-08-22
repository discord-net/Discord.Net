namespace Discord.Models;

public interface INestedChannelModel : IGuildChannelModel
{
    Optional<Snowflake?> ParentId { get; }
}