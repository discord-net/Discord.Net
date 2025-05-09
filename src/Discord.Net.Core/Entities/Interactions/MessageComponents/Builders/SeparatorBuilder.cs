namespace Discord;

public class SeparatorBuilder : IMessageComponentBuilder
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.Separator;

    /// <summary>
    ///     Gets or sets whether the component is a divider.
    /// </summary>
    public bool? IsDivider { get; set; }

    /// <summary>
    ///     Gets or sets the spacing of the separator.
    /// </summary>
    public SeparatorSpacingSize? Spacing { get; set; }

    /// <inheritdoc/>
    public int? Id { get; set; }

    /// <summary>
    ///     Initializes a new <see cref="SeparatorBuilder"/>.
    /// </summary>
    public SeparatorBuilder(bool isDivider = true, SeparatorSpacingSize spacing = SeparatorSpacingSize.Small)
    {
        IsDivider = isDivider;
        Spacing = spacing;
    }

    /// <summary>
    ///     Initializes a new <see cref="SeparatorBuilder"/> from existing component.
    /// </summary>
    public SeparatorBuilder(SeparatorComponent separator)
    {
        IsDivider = separator.IsDivider;
        Spacing = separator.Spacing;
        Id = separator.Id;
    }

    /// <summary>
    ///     Sets whether the component is a divider.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SeparatorBuilder WithIsDivider(bool? isDivider)
    {
        IsDivider = isDivider;
        return this;
    }

    /// <summary>
    ///     Sets the spacing of the separator.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SeparatorBuilder WithSpacing(SeparatorSpacingSize? spacing)
    {
        Spacing = spacing;
        return this;
    }

    /// <inheritdoc cref="IMessageComponentBuilder.Build"/>
    public SeparatorComponent Build()
    {
        return new(IsDivider, Spacing, Id);
    }

    /// <inheritdoc/>
    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
