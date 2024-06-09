using System;
namespace Discord.Models
{
    public interface IStageInstanceModel : IEntityModel<ulong>
    {
        ulong GuildId { get; }
        ulong ChannelId { get; }
        string Topic { get; }
        int PrivacyLevel { get; }
        bool DiscoverableDisabled { get; }
        ulong? EventId { get; }
    }
}

