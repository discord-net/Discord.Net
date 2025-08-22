namespace Discord.Models;

public interface IPruneCountResponseModel : IModel
{
    int Pruned { get; }
}