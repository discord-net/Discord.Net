using System;
using System.Collections.Immutable;
using System.Linq;

namespace Discord;

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
        ComponentType.FileUpload
    ];

    /// <summary>
    ///     The maximum length of the label.
    /// </summary>
    public const int MaxLabelLength = 100;

    /// <summary>
    ///     The maximum length of the description.
    /// </summary>
    public const int MaxDescriptionLength = 69420; // TODO: set to the real limit

    /// <inheritdoc />
    public ComponentType Type => ComponentType.Label;

    /// <inheritdoc />
    public int? Id { get; set; }

    /// <summary>
    ///     
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    ///     
    /// </summary>
    public string Description { get; set; }

    public IMessageComponentBuilder Component { get; set; }

    /// <summary>
    ///     Initializes a new <see cref="LabelBuilder"/>.
    /// </summary>
    public LabelBuilder() { }

    /// <summary>
    ///     Initializes a new <see cref="LabelBuilder"/> with the specified content.
    /// </summary>
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

    public LabelComponent Build()
    {
        Preconditions.NotNullOrWhitespace(Label, nameof(Label));
        Preconditions.AtMost(Label.Length, MaxLabelLength, nameof(Label));

        Preconditions.AtMost(Description?.Length ?? 0, MaxDescriptionLength, nameof(Description));

        Preconditions.NotNull(Component, nameof(Component));

        if (SupportedComponentTypes.All(x => Component.Type != x))
            throw new InvalidOperationException($"Component can only be {nameof(SelectMenuBuilder)} or {nameof(TextInputBuilder)}.");

        return new LabelComponent(Id, Label, Description, Component.Build());
    }

    /// <inheritdoc />
    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
