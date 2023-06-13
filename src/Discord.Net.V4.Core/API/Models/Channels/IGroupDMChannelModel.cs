namespace Discord.Models
{
    public interface IGroupDMChannelModel : IAudioChannelModel
    {
        ulong[] Recipients { get; }
    }
}
