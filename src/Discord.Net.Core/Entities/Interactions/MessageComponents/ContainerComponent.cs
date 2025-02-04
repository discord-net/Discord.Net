using System.Collections.Generic;

namespace Discord;

public class ContainerComponent : IMessageComponent
{
    public ComponentType Type => ComponentType.Container;

    public int? Id { get; }

    public IReadOnlyCollection<IMessageComponent> Components { get; }

    public uint? AccentColor { get; }

    public bool? IsSpoiler { get; }

    internal ContainerComponent(IReadOnlyCollection<IMessageComponent> components, uint? accentColor, bool? isSpoiler, int? id = null)
    {
        Components = components;
        AccentColor = accentColor;
        IsSpoiler = isSpoiler;
        Id = id;
    }
}
