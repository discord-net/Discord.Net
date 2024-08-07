namespace Discord.Models;

public interface IMessageComponentDataModel : IInteractionDataModel
{
    string CustomId { get; }
    int ComponentType { get; }
    string[]? Values { get; }
    IResolvedDataModel? Resolved { get; }
}
