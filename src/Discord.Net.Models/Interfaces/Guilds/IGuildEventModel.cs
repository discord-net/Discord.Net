namespace Discord.Models
{
    public interface IGuildEventModel : IEntityModel<ulong>
    {
        ulong GuildId { get; }
        ulong? ChannelId { get; }
        ulong CreatorId { get; }
        string Name { get; }
        string? Description { get; }
        DateTimeOffset StartTime { get; }
        DateTimeOffset? EndTime { get; }
        int PrivacyLevel { get; }
        int Status { get; }
        int Type { get; }
        ulong? EntityId { get; }
        int? UserCount { get; }
        string? Image { get; }

        // metadata
        string? Location { get; }
    }
}

