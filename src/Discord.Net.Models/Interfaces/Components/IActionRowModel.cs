namespace Discord.Models;

[ModelEquality]
public partial interface IActionRowModel : IMessageComponentModel
{
    IEnumerable<IMessageComponentModel> Components { get; }
}
