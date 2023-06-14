namespace Discord.Models
{
    public interface IVoiceStateModel : IEntityModel<string>
    {
        ulong UserId { get; }
        ulong ChannelId { get; }
        ulong? GuildId { get; }
        VoiceStateFlags Flags { get; }
        DateTimeOffset RequestToSpeak { get; }
    }

    public enum VoiceStateFlags
    {
        SelfMute = 1 << 0,
        SelfDeaf = 1 << 1,
        Deaf = 1 << 2,
        Mute = 1 << 3,
        SelfStream = 1 << 4,
        SelfVideo = 1 << 5,
        Supressed = 1 << 6
    }
}
