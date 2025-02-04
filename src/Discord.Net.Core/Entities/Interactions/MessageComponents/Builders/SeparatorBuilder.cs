namespace Discord;

public class SeparatorBuilder : IMessageComponentBuilder
{
    public ComponentType Type => ComponentType.Separator;

    public bool? IsDivider { get; set; }

    public SeparatorSpacingSize? Spacing { get; set; }

    public int? Id { get; set; }

    public SeparatorBuilder WithIsDivider(bool? isDivider)
    {
        IsDivider = isDivider;
        return this;
    }

    public SeparatorBuilder WithSpacing(SeparatorSpacingSize? spacing)
    {
        Spacing = spacing;
        return this;
    }

    public SeparatorBuilder WithId(int? id)
    {
        Id = id;
        return this;
    }

    public SeparatorComponent Build()
    {
        return new(IsDivider, Spacing, Id);
    }

    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
