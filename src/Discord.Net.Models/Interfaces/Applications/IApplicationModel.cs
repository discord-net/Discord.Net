namespace Discord.Models;

[ModelEquality]
public partial interface IApplicationModel : IEntityModel<ulong>
{
    string Name { get; }
    string? Icon { get; }
    string Description { get; }
    ulong? BotId { get; }
}
