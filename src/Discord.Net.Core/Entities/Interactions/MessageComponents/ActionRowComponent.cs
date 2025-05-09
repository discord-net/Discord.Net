using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents a <see cref="IMessageComponent"/> Row for child components to live in.
/// </summary>
public class ActionRowComponent : INestedComponent, IMessageComponent
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.ActionRow;

    /// <inheritdoc/>
    public int? Id { get; internal set; }

    /// <summary>
    ///     Gets the child components in this row.
    /// </summary>
    public IReadOnlyCollection<IMessageComponent> Components { get; internal set; }

    /// <summary>
    ///     Converts a <see cref="ActionRowComponent"/> to a <see cref="ActionRowBuilder"/>.
    /// </summary>
    public ActionRowBuilder ToBuilder()
        => new(this);

    internal ActionRowComponent() { }

    internal ActionRowComponent(IReadOnlyCollection<IMessageComponent> components, int? id)
    {
        Components = components;
        Id = id;
    }

    /// <inheritdoc />
    IMessageComponentBuilder IMessageComponent.ToBuilder() => ToBuilder();
}
