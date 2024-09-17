namespace Discord.Models;

[ModelEquality]
public partial interface IGuildTemplateModel : IEntityModel<string>
{
    string Name { get; }
    string? Description { get; }
    int UsageCount { get; }
    ulong CreatorId { get; }
    DateTimeOffset CreatedAt { get; }
    DateTimeOffset UpdatedAt { get; }
    ulong SourceGuildId { get; }
    bool? IsDirty { get; }
}