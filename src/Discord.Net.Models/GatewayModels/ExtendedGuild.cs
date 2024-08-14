using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Unavailable), false)]
public sealed partial class ExtendedGuild : GuildCreated, IExtendedGuild
{
    [JsonIgnore, JsonExtend]
    public required Guild Guild { get; set; }

    [JsonPropertyName("joined_at")]
    public DateTimeOffset JoinedAt { get; set; }

    [JsonPropertyName("large")]
    public bool IsLarge { get; set; }

    [JsonPropertyName("member_count")]
    public int MemberCount { get; set; }

    [JsonPropertyName("voice_states")]
    public required PartialVoiceState[] VoiceStates { get; set; }

    [JsonPropertyName("members")]
    public required GuildMember[] Members { get; set; }

    [JsonPropertyName("channels")]
    public required GuildChannelBase[] Channels { get; set; }

    [JsonPropertyName("threads")]
    public required ThreadChannelBase[] Threads { get; set; }

    [JsonPropertyName("presences")]
    public required Presence[] Presences { get; set; }

    [JsonPropertyName("stage_instances")]
    public required StageInstance[] StageInstances { get; set; }

    [JsonPropertyName("guild_scheduled_events")]
    public required GuildScheduledEvent[] GuildScheduledEvents { get; set; }

    IEnumerable<IPartialVoiceStateModel> IExtendedGuild.VoiceStates => VoiceStates;
    IEnumerable<IMemberModel> IExtendedGuild.Members => Members;

    IEnumerable<IGuildChannelModel> IExtendedGuild.Channels => Channels;

    IEnumerable<IThreadChannelModel> IExtendedGuild.Threads => Threads;

    IEnumerable<IPresenceModel> IExtendedGuild.Presences => Presences;
    IEnumerable<IStageInstanceModel> IExtendedGuild.StageInstances => StageInstances;
    IEnumerable<IGuildScheduledEventModel> IExtendedGuild.GuildScheduledEvents => GuildScheduledEvents;
}
