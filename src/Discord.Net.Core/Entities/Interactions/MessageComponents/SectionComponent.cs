using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents a section component.
/// </summary>
public class SectionComponent : INestedComponent, IMessageComponent
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.Section;

    /// <inheritdoc/>
    public int? Id { get; }

    /// <summary>
    ///     Gets the components in this section.
    /// </summary>
    public IReadOnlyCollection<IMessageComponent> Components { get; }

    /// <summary>
    ///     Gets the accessory of this section.
    /// </summary>
    public IMessageComponent Accessory { get; }

    /// <summary>
    ///     Converts a <see cref="SectionComponent"/> to a <see cref="SectionBuilder"/>.
    /// </summary>
    public SectionBuilder ToBuilder()
        => new(this);

    internal SectionComponent(int? id, IReadOnlyCollection<IMessageComponent> components, IMessageComponent accessory)
    {
        Id = id;
        Components = components;
        Accessory = accessory;
    }

    /// <inheritdoc />
    IMessageComponentBuilder IMessageComponent.ToBuilder() => ToBuilder();
}
