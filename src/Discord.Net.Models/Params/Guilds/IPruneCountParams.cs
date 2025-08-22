namespace Discord.Models;

public interface IPruneCountParams : IModel
{
    Optional<int> Days { get; }
    Optional<CSVString<ulong>> Roles { get; }
}