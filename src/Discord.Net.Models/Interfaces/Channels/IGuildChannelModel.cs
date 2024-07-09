namespace Discord.Models;

public interface IGuildChannelModel : IChannelModel
{
    ulong GuildId { get; }
    ulong? ParentId { get; }
    int Position { get; }
    IEnumerable<IOverwriteModel> Permissions { get; }
    int? Flags { get; }
}
