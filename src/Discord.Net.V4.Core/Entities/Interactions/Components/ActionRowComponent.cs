using System.Collections.Immutable;

namespace Discord;

public sealed class ActionRowComponent : IMessageComponent
{
    internal ActionRowComponent(IEnumerable<IMessageComponent> components)
    {
        Components = components.ToImmutableArray();
    }

    /// <summary>
    ///     Gets the child components in this row.
    /// </summary>
    public IReadOnlyCollection<IMessageComponent> Components { get; }

    /// <inheritdoc />
    public ComponentType Type
        => ComponentType.ActionRow;

    /// <inheritdoc />
    /// <remarks>
    ///     This property is always <see langword="null" /> for <see cref="ActionRowComponent" />.
    /// </remarks>
    public string? CustomId
        => null;
}
