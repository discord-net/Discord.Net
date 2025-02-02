using System.Collections.Generic;

namespace Discord;

public class SectionComponent : IMessageComponent
{
    public ComponentType Type => ComponentType.Section;

    public int? Id { get; }

    public IReadOnlyCollection<IMessageComponent> Components { get; }

    public IMessageComponent Accessory { get; }

    internal SectionComponent(int? id, IReadOnlyCollection<IMessageComponent> components, IMessageComponent accessory)
    {
        Id = id;
        Components = components;
        Accessory = accessory;
    }
}
