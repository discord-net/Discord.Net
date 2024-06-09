namespace Discord.Models;

public interface IApplicationModel : IEntityModel<ulong>
{
    string Name { get; }
    string? Icon { get; }
    string Description { get; }
    ulong? BotId { get; }
}
