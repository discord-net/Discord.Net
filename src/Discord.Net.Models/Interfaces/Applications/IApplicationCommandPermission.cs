namespace Discord.Models;

public interface IApplicationCommandPermission : IEntityModel<ulong>
{
    int Type { get; }
    bool Permission { get; }
}
