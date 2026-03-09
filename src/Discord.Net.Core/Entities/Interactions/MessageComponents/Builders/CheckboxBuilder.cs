using System;

namespace Discord;

/// <summary>
///     Represents a builder used to create a <see cref="CheckboxComponent" />.
/// </summary>
public class CheckboxBuilder : IInteractableComponentBuilder
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.Checkbox;

    /// <inheritdoc />
    public int? Id { get; set; }

    /// <summary>
    ///     Gets or sets the custom id of the current file upload.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length exceeds <see cref="ModalComponentBuilder.MaxCustomIdLength"/>.</exception>
    /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length subceeds 1.</exception>
    public string CustomId
    {
        get;
        set
        {
            if (value is not null)
            {
                Preconditions.AtLeast(value.Length, 1, nameof(CustomId));
                Preconditions.AtMost(value.Length, ModalComponentBuilder.MaxCustomIdLength, nameof(CustomId));
            }

            field = value;
        }
    }

    /// <summary>
    ///     Gets or sets the default state of the checkbox.
    /// </summary>
    public bool? DefaultState { get; set; }

    /// <summary>
    ///     Sets the custom id of the current checkbox.
    /// </summary>
    /// <param name="customId">The id to use for the current checkbox.</param>
    /// <inheritdoc cref="CustomId"/>
    /// <returns>The current builder.</returns>
    public CheckboxBuilder WithCustomId(string customId)
    {
        CustomId = customId;
        return this;
    }

    /// <summary>
    ///     Sets the default checked state of the checkbox and returns the current builder instance.
    /// </summary>
    /// <param name="defaultState"><see langword="true"/>> to set the checkbox as checked by default; otherwise, <see langword="false"/>.</param>
    /// <returns>The current instance of the CheckboxBuilder, enabling method chaining.</returns>
    public CheckboxBuilder WithDefaultState(bool? defaultState)
    {
        DefaultState = defaultState;
        return this;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CheckboxBuilder"/>.
    /// </summary>
    public CheckboxBuilder() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CheckboxBuilder"/>.
    /// </summary>
    /// <param name="customId">The custom id of the current checkbox.</param>
    /// <param name="defaultState">The default state of the checkbox.</param>
    /// <param name="id">The id for the component.</param>
    public CheckboxBuilder(string customId, bool? defaultState = null, int? id = null)
    {
        CustomId = customId;
        DefaultState = defaultState;
        Id = id;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CheckboxBuilder"/> class from an existing <see cref="CheckboxComponent"/>.
    /// </summary>
    /// <param name="checkbox">The component.</param>
    public CheckboxBuilder(CheckboxComponent checkbox)
    {
        CustomId = checkbox.CustomId;
        DefaultState = checkbox.DefaultState;
        Id = checkbox.Id;
    }

    /// <inheritdoc cref="IMessageComponentBuilder.Build" />
    public CheckboxComponent Build()
    {
        Preconditions.NotNullOrWhitespace(CustomId, nameof(CustomId));
        
        return new (Id, CustomId, DefaultState);
    }

    /// <inheritdoc/>
    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
