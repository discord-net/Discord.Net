using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord;

/// <summary>
///     Represents a builder used to create a <see cref="RadioGroupComponent" />.
/// </summary>
public class RadioGroupBuilder : IInteractableComponentBuilder
{
    /// <summary>
    ///     Gets the minimum amount of options a radio group must have.
    /// </summary>
    public const int MinOptionCount = 2;

    /// <summary>
    ///     Gets the maximum amount of options a radio group can have.
    /// </summary>
    public const int MaxOptionCount = 10;

    /// <inheritdoc/>
    public ComponentType Type => ComponentType.RadioGroup;

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
    ///     Gets or sets the options for this radio group.
    /// </summary>
    public List<RadioGroupOptionProperties> Options
    {
        get;
        set
        {
            if (value != null)
                Preconditions.AtMost(value.Count, MaxOptionCount, nameof(Options));
            field = value;
        }
    } = [];

    /// <summary>
    ///     Gets or sets a value indicating whether the current radio group has to be filled in before submitting the modal (defaults to <see langword="true"></see>).
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RadioGroupBuilder"/>.
    /// </summary>
    public RadioGroupBuilder() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RadioGroupBuilder"/>.
    /// </summary>
    /// <param name="customId">The custom id of the current radio group.</param>
    /// <param name="options">The options for this radio group.</param>
    /// <param name="isRequired">Whether the current radio group requires selection before submitting the modal.</param>
    /// <param name="id">The id for the component.</param>
    public RadioGroupBuilder(string customId, IEnumerable<RadioGroupOptionProperties> options = null, bool isRequired = true, int? id = null)
    {
        CustomId = customId;
        Options = options?.ToList();
        IsRequired = isRequired;
        Id = id;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RadioGroupBuilder"/> class from an existing <see cref="RadioGroupComponent"/>.
    /// </summary>
    /// <param name="radioGroup">The component.</param>
    public RadioGroupBuilder(RadioGroupComponent radioGroup)
    {
        CustomId = radioGroup.CustomId;
        IsRequired = radioGroup.IsRequired;
        Options = radioGroup.Options.Select(x => new RadioGroupOptionProperties(x.Value, x.Label, x.Description, x.IsDefault)).ToList();
        Id = radioGroup.Id;
    }

    /// <summary>
    ///     Sets the custom id of the current radio group.
    /// </summary>
    /// <param name="customId">The id to use for the current radio group.</param>
    /// <inheritdoc cref="CustomId"/>
    /// <returns>The current builder.</returns>
    public RadioGroupBuilder WithCustomId(string customId)
    {
        CustomId = customId;
        return this;
    }

    /// <summary>
    ///     Sets whether the current radio group requires selection before submitting the modal.
    /// </summary>
    /// <param name="isRequired">Sets whether the current radio group requires selection before submitting the modal.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public RadioGroupBuilder WithRequired(bool isRequired)
    {
        IsRequired = isRequired;
        return this;
    }

    /// <summary>
    ///     Sets the options for this radio group.
    /// </summary>
    /// <param name="options">The options to set.</param>
    /// <exception cref="ArgumentException" accessor="set"><see cref="Options"/> count exceeds <see cref="MaxOptionCount"/>.</exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public RadioGroupBuilder WithOptions(params List<RadioGroupOptionProperties> options)
    {
        Options = options;
        return this;
    }

    /// <summary>
    ///     Adds one option to the radio group.
    /// </summary>
    /// <param name="option">The option to add.</param>
    /// <exception cref="InvalidOperationException">Options count reached <see cref="MaxOptionCount"/>.</exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public RadioGroupBuilder AddOption(RadioGroupOptionProperties option)
    {
        Options ??= new();

        if (Options.Count >= MaxOptionCount)
            throw new InvalidOperationException($"Options count reached {MaxOptionCount}.");

        Options.Add(option);
        return this;
    }

    /// <summary>
    ///     Adds one option to the radio group.
    /// </summary>
    /// <param name="label">The label for this option.</param>
    /// <param name="value">The value of this option.</param>
    /// <param name="description">The description of this option </param>
    /// <param name="isDefault">Whether this option is selected by default.</param>
    /// <exception cref="InvalidOperationException">Options count reached <see cref="MaxOptionCount"/>.</exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public RadioGroupBuilder AddOption(string label, string value, string description = null, bool isDefault = false)
    {
        AddOption(new RadioGroupOptionProperties(value, label, description, isDefault));
        return this;
    }

    /// <inheritdoc cref="IMessageComponentBuilder.Build" />
    public RadioGroupComponent Build()
    {
        Preconditions.NotNullOrWhitespace(CustomId, nameof(CustomId));

        Preconditions.AtLeast(Options?.Count ?? 0, MinOptionCount, nameof(Options));
        Preconditions.AtMost(Options.Count, MaxOptionCount, nameof(Options));

        return new RadioGroupComponent(Id,
            CustomId,
            Options.Select(x => new RadioGroupOption(x.Value, x.Label, x.Description, x.IsDefault)).ToImmutableArray(),
            IsRequired);
    }

    /// <inheritdoc/>
    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
