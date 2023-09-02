using System;
namespace Discord.Models
{
    public interface IStageInstanceModel : IEntityModel<ulong>
    {
        ulong GuildId { get; }
        ulong ChannelId { get; }
        string Topic { get; }
        StagePrivacyLevel PrivacyLevel { get; }
        bool DiscoverableDisabled { get; }
        ulong? EventId { get; }
    }
}

