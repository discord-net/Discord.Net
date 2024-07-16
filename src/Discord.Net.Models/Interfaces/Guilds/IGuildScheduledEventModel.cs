namespace Discord.Models
{
    [ModelEquality]
    public partial interface IGuildScheduledEventModel : IEntityModel<ulong>
    {
        ulong GuildId { get; }
        ulong? ChannelId { get; }
        ulong CreatorId { get; }
        string Name { get; }
        string? Description { get; }
        DateTimeOffset ScheduledStartTime { get; }
        DateTimeOffset? ScheduledEndTime { get; }
        int PrivacyLevel { get; }
        int Status { get; }
        int EntityType { get; }
        ulong? EntityId { get; }
        int? UserCount { get; }
        string? Image { get; }

        // metadata
        string? Location { get; }
    }
}
