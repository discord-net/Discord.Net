namespace Discord.Models;

public interface IBanModel
{
    string? Reason { get; }
    ulong UserId { get; }
}
