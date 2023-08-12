using Newtonsoft.Json;
using System;
namespace Discord.WebSocket.Cache
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

