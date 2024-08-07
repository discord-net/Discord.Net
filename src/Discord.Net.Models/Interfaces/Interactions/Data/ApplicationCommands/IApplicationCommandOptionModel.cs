namespace Discord.Models;

public interface IApplicationCommandOptionModel
{
    string Name { get; }
    int Type { get; }
    object? Value { get; }
    IEnumerable<IApplicationCommandOptionModel>? Options { get; }
    bool? IsFocused { get; }
}
