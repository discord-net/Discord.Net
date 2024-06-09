namespace Discord.Models;

public interface IGuildChannelModel : IChannelModel
{
    ulong? ParentId { get; }
    int Position { get; }
    IEnumerable<IOverwriteModel> Permissions { get; }
    int? Flags { get; }
}
