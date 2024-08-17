using System.Diagnostics.CodeAnalysis;
using Discord.Models.Json;

namespace Discord.Models;

[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public interface IExtendedGuild : 
    IGuildCreatePayloadData, 
    IGuildModel,
    IModelSourceOfMultiple<IPartialVoiceStateModel>,
    IModelSourceOfMultiple<IMemberModel>,
    IModelSourceOfMultiple<IGuildChannelModel>,
    IModelSourceOfMultiple<IThreadChannelModel>,
    IModelSourceOfMultiple<IPresenceModel>,
    IModelSourceOfMultiple<IStageInstanceModel>,
    IModelSourceOfMultiple<IGuildScheduledEventModel>
{
    DateTimeOffset JoinedAt { get; }
    bool IsLarge { get; }
    int MemberCount { get; }
    IEnumerable<IPartialVoiceStateModel> VoiceStates { get; }
    IEnumerable<IMemberModel> Members { get; }
    IEnumerable<IGuildChannelModel> Channels { get; }
    IEnumerable<IThreadChannelModel> Threads { get; }
    IEnumerable<IPresenceModel> Presences { get; }
    IEnumerable<IStageInstanceModel> StageInstances { get; }
    IEnumerable<IGuildScheduledEventModel> GuildScheduledEvents { get; }

    IEnumerable<IPartialVoiceStateModel> IModelSourceOfMultiple<IPartialVoiceStateModel>.GetModels()
        => VoiceStates;

    IEnumerable<IMemberModel> IModelSourceOfMultiple<IMemberModel>.GetModels()
        => Members;

    IEnumerable<IGuildChannelModel> IModelSourceOfMultiple<IGuildChannelModel>.GetModels()
        => Channels;

    IEnumerable<IThreadChannelModel> IModelSourceOfMultiple<IThreadChannelModel>.GetModels()
        => Threads;

    IEnumerable<IPresenceModel> IModelSourceOfMultiple<IPresenceModel>.GetModels()
        => Presences;

    IEnumerable<IStageInstanceModel> IModelSourceOfMultiple<IStageInstanceModel>.GetModels()
        => StageInstances;

    IEnumerable<IGuildScheduledEventModel> IModelSourceOfMultiple<IGuildScheduledEventModel>.GetModels()
        => GuildScheduledEvents;

    IEnumerable<IModel> IModelSource.GetDefinedModels()
        => [..VoiceStates, ..Members, ..Channels, ..Threads, ..Presences, ..StageInstances, ..GuildScheduledEvents];
}
