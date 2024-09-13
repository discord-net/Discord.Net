namespace Discord.Models;

public interface IApplicationCommandInteractionOptionModel
{
    string Name { get; }
    int Type { get; }
    object? Value { get; }
    IEnumerable<IApplicationCommandInteractionOptionModel>? Options { get; }
    bool? IsFocused { get; }
}
