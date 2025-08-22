namespace Discord.Models;

public interface IThreadableChannelModel : IGuildChannelModel, INestedChannelModel
{
    DefaultAutoArchiveDuration DefaultAutoArchiveDuration { get; }
}