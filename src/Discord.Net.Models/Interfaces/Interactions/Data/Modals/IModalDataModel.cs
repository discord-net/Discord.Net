namespace Discord.Models;

public interface IModalDataModel : IInteractionDataModel
{
    string CustomId { get; }
    IEnumerable<IMessageComponentModel> Components { get; }
}
