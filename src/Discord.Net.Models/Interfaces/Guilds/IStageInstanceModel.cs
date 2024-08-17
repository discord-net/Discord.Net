using System;

namespace Discord.Models;

[ModelEquality]
public partial interface IStageInstanceModel : IEntityModel<ulong>
{
    [Link<IGuildModel>]
    ulong GuildId { get; }
    
    [Link<IChannelModel>]
    ulong ChannelId { get; }
    
    string Topic { get; }
    int PrivacyLevel { get; }
    bool DiscoverableDisabled { get; }
    ulong? EventId { get; }
}