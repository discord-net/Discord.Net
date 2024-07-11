namespace Discord.Models;

[ModelEquality]
public partial interface IVoiceStateModel : IEntityModel<string>
{
    ulong UserId { get; }
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
}
