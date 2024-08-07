namespace Discord.Models;

public interface IApplicationCommandDataModel : IInteractionDataModel
{
    ulong Id { get; }
    string Name { get; }
    int Type { get; }
    IResolvedDataModel? Resolved { get; }
    IEnumerable<IApplicationCommandOptionModel>? Options { get; }
    ulong? GuildId { get; }
    ulong? TargetId { get; }
}
