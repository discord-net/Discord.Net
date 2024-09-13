namespace Discord.Models;

public interface IApplicationCommandInteractionDataModel : IInteractionDataModel
{
    ulong Id { get; }
    string Name { get; }
    int Type { get; }
    IResolvedDataModel? Resolved { get; }
    IEnumerable<IApplicationCommandInteractionOptionModel>? Options { get; }
    ulong? GuildId { get; }
    ulong? TargetId { get; }
}
