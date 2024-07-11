namespace Discord.Models;

[ModelEquality]
public partial interface IThreadableChannelModel : IGuildChannelModel
{
    int DefaultAutoArchiveDuration { get; }
    int? DefaultThreadRateLimitPerUser { get; }
}
