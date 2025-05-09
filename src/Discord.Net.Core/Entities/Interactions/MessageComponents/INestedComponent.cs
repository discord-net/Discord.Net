using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents a <see cref="IMessageComponent"/> containing child components.
/// </summary>
public interface INestedComponent
{
    /// <summary>
    ///     Gets the child components in this container.
    /// </summary>
    IReadOnlyCollection<IMessageComponent> Components { get; }
}
