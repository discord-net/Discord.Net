using System;
using System.Collections.Immutable;

namespace Discord;

/// <summary>
///     Represents a class used to build <see cref="LabelComponent"/>'s.
/// </summary>
public class LabelBuilder : IMessageComponentBuilder
{
    /// <inheritdoc cref="IComponentContainer.SupportedComponentTypes"/>
    public ImmutableArray<ComponentType> SupportedComponentTypes { get; } =
    [
        ComponentType.SelectMenu,
        ComponentType.TextInput,
        ComponentType.UserSelect,
        ComponentType.RoleSelect,
        ComponentType.MentionableSelect,
        ComponentType.ChannelSelect,
        ComponentType.FileUpload,
        ComponentType.RadioGroup,
        ComponentType.CheckboxGroup,
        ComponentType.Checkbox
    ];

    /// <summary>
    ///     The maximum length of the label.
    /// </summary>
    public const int MaxLabelLength = 45;

    /// <summary>
    ///     The maximum length of the description.
    /// </summary>
    public const int MaxDescriptionLength = 100;

    /// <inheritdoc />
    public ComponentType Type => ComponentType.Label;

    /// <inheritdoc />
    public int? Id { get; set; }

    /// <summary>
    ///     Gets or sets the label text.
    /// </summary>
    public string Label
    {
        get;
        set
        {
            if (value is not null)
            {
                Preconditions.AtMost(value.Length, MaxLabelLength, nameof(Label));
            }

            field = value;
        }
    }

    /// <summary>
    ///     Gets or sets the description text for the label.
    /// </summary>
    public string Description
    {
        get;
        set
        {
            if (value is not null)
            {
                Preconditions.AtMost(value.Length, MaxDescriptionLength, nameof(Description));
            }

            field = value;
        }
    }

    /// <summary>
    ///     Gets or sets the component within the label.
    /// </summary>
    public IMessageComponentBuilder Component { get; set; }

    /// <summary>
    ///     Sets the label text.
    /// </summary>
    /// <param name="label">The label text.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public LabelBuilder WithLabel(string label)
    {
        Label = label;
        return this;
    }

    /// <summary>
    ///     Sets the description text for the label.
    /// </summary>
    /// <param name="description">The description text for the label.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public LabelBuilder WithDescription(string description)
    {
        Description = description;
        return this;
    }

    /// <summary>
    ///     Sets the component within the label.
    /// </summary>
    /// <param name="component">The component within the label.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public LabelBuilder WithComponent(IMessageComponentBuilder component)
    {
        Component = component;
        return this;
    }

    /// <summary>
    ///     Initializes a new <see cref="LabelBuilder"/>.
    /// </summary>
    public LabelBuilder() { }

    /// <summary>
    ///     Initializes a new <see cref="LabelBuilder"/> with the specified content.
    /// </summary>
    /// <param name="label">The label text.</param>
    /// <param name="component">The component within the label.</param>
    /// <param name="description">The description text for the label.</param>
    /// <param name="id">The id for the component.</param>
    public LabelBuilder(string label, IMessageComponentBuilder component, string description = null, int? id = null)
    {
        Id = id;
        Label = label;
        Component = component;
        Description = description;
    }

    /// <summary>
    ///     Initializes a new <see cref="LabelBuilder"/> from existing component.
    /// </summary>
    public LabelBuilder(LabelComponent label)
    {
        Label = label.Label;
        Description = label.Description;
        Id = label.Id;
        Component = label.Component.ToBuilder();
    }

    /// <inheritdoc cref="IMessageComponentBuilder.Build" />
    public LabelComponent Build()
    {
        Preconditions.NotNullOrWhitespace(Label, nameof(Label));
        Preconditions.AtMost(Label.Length, MaxLabelLength, nameof(Label));

        Preconditions.AtMost(Description?.Length ?? 0, MaxDescriptionLength, nameof(Description));

        Preconditions.NotNull(Component, nameof(Component));

        if (!SupportedComponentTypes.Contains(Component.Type))
            throw new InvalidOperationException($"Component can only be {nameof(SelectMenuBuilder)}, {nameof(TextInputBuilder)} or {nameof(FileUploadComponentBuilder)}.");

        return new LabelComponent(Id, Label, Description, Component.Build());
    }

    /// <inheritdoc />
    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
