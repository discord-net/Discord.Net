namespace Discord.Models.Json;

public class ThreadableChannelBase : GuildChannelBase, IThreadableChannelModel
{
    int IThreadableChannelModel.DefaultAutoArchiveDuration => ~DefaultAutoArchiveDuration;

    int? IThreadableChannelModel.DefaultThreadRateLimitPerUser => ~DefaultThreadRatelimitPerUser;
}
