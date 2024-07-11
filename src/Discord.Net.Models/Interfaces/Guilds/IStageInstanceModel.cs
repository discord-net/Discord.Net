using System;
namespace Discord.Models
{
    [ModelEquality]
public partial interface IStageInstanceModel : IEntityModel<ulong>
    {
        ulong GuildId { get; }
        ulong ChannelId { get; }
        string Topic { get; }
        int PrivacyLevel { get; }
        bool DiscoverableDisabled { get; }
        ulong? EventId { get; }
    }
}

