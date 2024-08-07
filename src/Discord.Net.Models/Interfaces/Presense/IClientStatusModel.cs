namespace Discord.Models;

public interface IClientStatusModel
{
    string? Desktop { get; }
    string? Mobile { get; }
    string? Web { get; }
}
