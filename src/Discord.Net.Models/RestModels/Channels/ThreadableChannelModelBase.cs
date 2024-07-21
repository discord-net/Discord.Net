namespace Discord.Models.Json;

public class ThreadableChannelModelBase : GuildChannelModelBase, IThreadableChannelModel
{
    int IThreadableChannelModel.DefaultAutoArchiveDuration => ~DefaultAutoArchiveDuration;

    int? IThreadableChannelModel.DefaultThreadRateLimitPerUser => ~DefaultThreadRatelimitPerUser;
}
