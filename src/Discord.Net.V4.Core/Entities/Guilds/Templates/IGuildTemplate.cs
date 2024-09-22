using Discord.Models;
using Discord.Rest;

namespace Discord;

[
    Refreshable(nameof(Routes.GetGuildTemplate)),
    FetchableOfMany(nameof(Routes.GetGuildTemplates))
]
public partial interface IGuildTemplate :
    IEntity<string, IGuildTemplateModel>,
    IGuildTemplateFromGuildActor
{
    IUserActor Creator { get; }
    
    string Name { get; }
    string? Description { get; }
    int UsageCount { get; }
    DateTimeOffset CreatedAt { get; }
    DateTimeOffset UpdatedAt { get; }
    bool IsDirty { get; }
}