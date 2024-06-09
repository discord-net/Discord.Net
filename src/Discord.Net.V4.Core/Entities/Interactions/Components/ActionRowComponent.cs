using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord;

public sealed class ActionRowComponent : IMessageComponent
{
    /// <inheritdoc/>
    public ComponentType Type
        => ComponentType.ActionRow;

    /// <inheritdoc/>
    /// <remarks>
    ///     This property is always <see langword="null"/> for <see cref="ActionRowComponent"/>.
    /// </remarks>
    public string? CustomId
        => null;

    /// <summary>
    ///     Gets the child components in this row.
    /// </summary>
    public IReadOnlyCollection<IMessageComponent> Components { get; }

    internal ActionRowComponent(IEnumerable<IMessageComponent> components)
    {
        Components = components.ToImmutableArray();
    }
}
