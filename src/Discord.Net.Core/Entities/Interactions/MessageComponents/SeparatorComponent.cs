namespace Discord;

public class SeparatorComponent : IMessageComponent
{
    public ComponentType Type => ComponentType.Separator;

    public int? Id { get; }

    public bool? IsDivider { get; }

    public SeparatorSpacingSize? Spacing { get; }

    internal SeparatorComponent(bool? isDivider, SeparatorSpacingSize? spacing, int? id = null)
    {
        IsDivider = isDivider;
        Spacing = spacing;
        Id = id;
    }
}
