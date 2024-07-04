namespace Discord.Models;

public interface IThreadableChannelModel : IGuildChannelModel
{
    int DefaultAutoArchiveDuration { get; }
    int? DefaultThreadRateLimitPerUser { get; }
}
