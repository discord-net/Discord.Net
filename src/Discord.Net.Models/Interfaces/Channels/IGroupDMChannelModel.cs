namespace Discord.Models;

[ModelEquality]
public partial interface IGroupDMChannelModel : IChannelModel
{
    ulong[] Recipients { get; }
}
