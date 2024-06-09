namespace Discord.Models;

public interface IGuildChannelModel : IChannelModel
{
    ulong? Parent { get; }
    int Position { get; }
    IOverwriteModel[] Permissions { get; }
    int? Flags { get; }
}
