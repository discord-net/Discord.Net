using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IGuildChannelModel : IChannelModel
{
    string Name { get; }
    Snowflake GuildId { get; }
    int Position { get; }
    IReadOnlyList<IOverwriteModel> PermissionOverwrites  { get; }
    
    Optional<PermissionBitSet> Permissions { get; }
}