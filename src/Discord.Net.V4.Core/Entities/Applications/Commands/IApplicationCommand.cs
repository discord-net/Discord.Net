using System.Globalization;
using Discord.Models;

namespace Discord;

public partial interface IApplicationCommand :
    ISnowflakeEntity<IApplicationCommandModel>,
    IApplicationCommandActor
{
    ApplicationCommandType Type { get; }
    
    IGuildActor? Guild { get; }
    
    string Name { get; }
    string Description { get; }
    
    IReadOnlyDictionary<CultureInfo, string> NameLocalization { get; }
    IReadOnlyDictionary<CultureInfo, string> DescriptionLocalization { get; }
    
    IReadOnlyCollection<ApplicationCommandOption> Options { get; }
    
    PermissionSet? DefaultMemberPermission { get; }
    
    bool IsNsfw { get; }
    
    IReadOnlySet<ApplicationIntegrationType> IntegrationTypes { get; }
    IReadOnlySet<InteractionContextType> ContextTypes { get; }
    
    ulong Version { get; }
    
    EntryPointCommandHandlerType? Handler { get; }
}