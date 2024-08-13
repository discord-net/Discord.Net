namespace Discord.Models;

[ModelEquality, HasPartialVariant]
public partial interface IVoiceStateModel : IEntityModel<ulong>
{
    [PartialIgnore]
    ulong UserId { get; }
    string SessionId { get; }
    ulong? ChannelId { get; }
    ulong? GuildId { get; }
    bool Deaf { get; }
    bool Mute { get; }
    bool SelfDeaf { get; }
    bool SelfMute { get; }
    bool? SelfStream { get; }
    bool SelfVideo { get; }
    bool Suppress { get; }
    DateTimeOffset? RequestToSpeakTimestamp { get; }

    ulong IEntityModel<ulong>.Id => UserId;
}
