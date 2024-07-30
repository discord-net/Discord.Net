namespace Discord.Models;

public interface IUnavailableGuild
{
    ulong Id { get; }
    bool Unavailable { get; }
}
