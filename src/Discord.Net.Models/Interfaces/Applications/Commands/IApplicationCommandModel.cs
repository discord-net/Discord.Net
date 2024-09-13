namespace Discord.Models;

[ModelEquality]
public partial interface IApplicationCommandModel : IEntityModel<ulong>
{
    int? Type { get; }
    ulong ApplicationId { get; }
    ulong? GuildId { get; }
    string Name { get; }
    IReadOnlyDictionary<string, string>? NameLocalization { get; }
    string Description { get; }
    IReadOnlyDictionary<string, string>? DescriptionLocalization { get; }
    string? DefaultMemberPermissions { get; }
    bool? IsNsfw { get; }
    int[]? IntegrationTypes { get; }
    int[]? Contexts { get; }
    ulong Version { get; }
}