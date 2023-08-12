namespace Discord.Models;

public interface IApplicationModel : IEntityModel<ulong>
{
    string Name { get; }
    string? Icon { get; }
    string Description { get; }
    IUserModel? Bot { get; }
}
