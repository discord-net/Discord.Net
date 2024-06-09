namespace Discord.Models;

public interface IGroupDMChannelModel : IAudioChannelModel, IChannelModel
{
    ulong[] Recipients { get; }
}
