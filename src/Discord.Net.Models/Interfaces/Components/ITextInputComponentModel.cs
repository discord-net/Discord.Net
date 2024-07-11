namespace Discord.Models;

[ModelEquality]
public partial interface ITextInputComponentModel : IMessageComponentModel
{
    string CustomId { get; }
    int Style { get; }
    string Label { get; }
    int? MinLength { get; }
    int? MaxLength { get; }
    bool? IsRequired { get; }
    string? Value { get; }
    string? Placeholder { get; }
}
