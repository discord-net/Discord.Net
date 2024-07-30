namespace Discord.Models;

[ModelEquality]
public partial interface IGuildChannelModel : IChannelModel
{
    string Name { get; }
    ulong GuildId { get; }
    ulong? ParentId { get; }
    int Position { get; }
    IEnumerable<IOverwriteModel> Permissions { get; }
    int? Flags { get; }
}
