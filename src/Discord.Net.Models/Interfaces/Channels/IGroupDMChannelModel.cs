namespace Discord.Models;

public interface IGroupDMChannelModel : IChannelModel
{
    ulong[] Recipients { get; }
}
