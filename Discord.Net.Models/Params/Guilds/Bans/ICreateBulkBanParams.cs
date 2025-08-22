namespace Discord.Models;

public interface ICreateBulkBanParams : IParametersModel
{
    ICollection<ulong> UserIds { get; }
    Optional<int> DeleteMessageSeconds { get; }
}