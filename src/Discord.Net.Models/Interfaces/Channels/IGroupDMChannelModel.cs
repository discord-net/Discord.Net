namespace Discord.Models;

[ModelEquality]
public partial interface IGroupDMChannelModel : IChannelModel
{
    IEnumerable<ulong> Recipients { get; }
}
