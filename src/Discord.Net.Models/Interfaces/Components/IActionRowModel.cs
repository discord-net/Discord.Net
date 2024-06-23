namespace Discord.Models;

public interface IActionRowModel : IMessageComponentModel
{
    IEnumerable<IMessageComponentModel> Components { get; }
}
