using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord;

/// <summary>
///     
/// </summary>
public class CheckboxGroupBuilder : IInteractableComponentBuilder
{
    /// <summary>
    ///     
    /// </summary>
    public const int MinOptionCount = 1;

    /// <summary>
    ///     
    /// </summary>
    public const int MaxOptionCount = 10;

    /// <inheritdoc/>
    public ComponentType Type => ComponentType.CheckboxGroup;

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
    ///     
    /// </summary>
    public List<CheckboxGroupOptionProperties> Options
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
    ///     Gets or sets the minimum number of options to be checked.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="MinValues"/> exceeds <see cref="MaxOptionCount"/>.</exception>
    /// <exception cref="ArgumentException" accessor="set"><see cref="MinValues"/> length subceeds 0.</exception>
    public int? MinValues
    {
        get;
        set
        {
            if (value is not null)
            {
                Preconditions.AtLeast(value.Value, 0, nameof(MinValues));
                Preconditions.AtMost(value.Value, MaxOptionCount, nameof(MinValues));
            }

            field = value;
        }
    }

    /// <summary>
    ///     Gets or sets the minimum number of options to be checked.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="MaxValues"/> exceeds <see cref="MaxOptionCount"/>.</exception>
    /// <exception cref="ArgumentException" accessor="set"><see cref="MaxValues"/> length subceeds <see cref="MinOptionCount"/>.</exception>
    public int? MaxValues
    {
        get;
        set
        {
            if (value is not null)
            {
                Preconditions.AtLeast(value.Value, MinOptionCount, nameof(MaxValues));
                Preconditions.AtMost(value.Value, MaxOptionCount, nameof(MaxValues));
            }

            field = value;
        }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the current checkbox group has to be filled in before submitting the modal (defaults to <see langword="true"></see>).
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CheckboxGroupBuilder"/>.
    /// </summary>
    public CheckboxGroupBuilder() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CheckboxGroupBuilder"/>.
    /// </summary>
    /// <param name="customId">The custom id of the current checkbox group.</param>
    /// <param name="options">The options for this checkbox group.</param>
    /// <param name="minValues">The minimum number of options that must be selected.</param>
    /// <param name="maxValues">The maximum number of options that can be selected.</param>
    /// <param name="isRequired">Whether the current checkbox group requires selection before submitting the modal.</param>
    /// <param name="id">The id for the component.</param>
    public CheckboxGroupBuilder(string customId, List<CheckboxGroupOptionProperties> options = null, int? minValues = null, int? maxValues = null, bool isRequired = true, int? id = null)
    {
        CustomId = customId;
        Options = options;
        MinValues = minValues;
        MaxValues = maxValues;
        IsRequired = isRequired;
        Id = id;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CheckboxGroupBuilder"/> class from an existing <see cref="CheckboxGroupComponent"/>.
    /// </summary>
    /// <param name="checkboxGroup">The component.</param>
    public CheckboxGroupBuilder(CheckboxGroupComponent checkboxGroup)
    {
        CustomId = checkboxGroup.CustomId;
        MinValues = checkboxGroup.MinValues;
        MaxValues = checkboxGroup.MaxValues;
        IsRequired = checkboxGroup.IsRequired;
        Options = checkboxGroup.Options.Select(x => new CheckboxGroupOptionProperties(x.Value, x.Label, x.Description ,x.DefaultState)).ToList();
        Id = checkboxGroup.Id;
    }

    /// <summary>
    ///     Sets the custom id of the current checkbox.
    /// </summary>
    /// <param name="customId">The id to use for the current checkbox.</param>
    /// <inheritdoc cref="CustomId"/>
    /// <returns>The current builder.</returns>
    public CheckboxGroupBuilder WithCustomId(string customId)
    {
        CustomId = customId;
        return this;
    }

    /// <summary>
    ///     Sets the minimum number of options that must be selected.
    /// </summary>
    /// <param name="minValues">The minimum number of options that must be selected.</param>
    /// <inheritdoc cref="MinValues"/>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public CheckboxGroupBuilder WithMinValues(int? minValues)
    {
        MinValues = minValues;
        return this;
    }

    /// <summary>
    ///     Sets the maximum number of options that can be selected.
    /// </summary>
    /// <param name="maxValues">The maximum number of options that must be selected.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public CheckboxGroupBuilder WithMaxValues(int? maxValues)
    {
        MaxValues = maxValues;
        return this;
    }

    /// <summary>
    ///     Sets whether the current checkbox group requires selection before submitting the modal.
    /// </summary>
    /// <param name="isRequired">Sets whether the current checkbox group requires selection before submitting the modal.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public CheckboxGroupBuilder WithRequired(bool isRequired)
    {
        IsRequired = isRequired;
        return this;
    }

    /// <summary>
    ///     Sets the options for this checkbox group.
    /// </summary>
    /// <param name="options">The options to set.</param>
    /// <exception cref="ArgumentException" accessor="set"><see cref="Options"/> count exceeds <see cref="MaxOptionCount"/>.</exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public CheckboxGroupBuilder WithOptions(params List<CheckboxGroupOptionProperties> options)
    {
        Options = options;
        return this;
    }

    /// <summary>
    ///     Adds one option to the checkbox group.
    /// </summary>
    /// <param name="option">The option to add.</param>
    /// <exception cref="InvalidOperationException">Options count reached <see cref="MaxOptionCount"/>.</exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public CheckboxGroupBuilder AddOption(CheckboxGroupOptionProperties option)
    {
        Options ??= new();

        if (Options.Count >= MaxOptionCount)
            throw new InvalidOperationException($"Options count reached {MaxOptionCount}.");

        Options.Add(option);
        return this;
    }

    /// <summary>
    ///     Adds one option to the checkbox group.
    /// </summary>
    /// <param name="label">The label for this option.</param>
    /// <param name="value">The value of this option.</param>
    /// <param name="description">The description of this option </param>
    /// <param name="defaultState">Whether this option is checked by default.</param>
    /// <exception cref="InvalidOperationException">Options count reached <see cref="MaxOptionCount"/>.</exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public CheckboxGroupBuilder AddOption(string label, string value, string description = null, bool defaultState = false)
    {
        AddOption(new CheckboxGroupOptionProperties(value, label, description, defaultState));
        return this;
    }

    /// <inheritdoc cref="IMessageComponentBuilder.Build" />
    public CheckboxGroupComponent Build()
    {
        Preconditions.NotNullOrWhitespace(CustomId, nameof(CustomId));

        if (MinValues is not null && MaxValues is not null)
            Preconditions.AtLeast(MaxValues.Value, MinValues.Value, nameof(MaxValues));

        Preconditions.AtLeast(MinValues ?? 0, 0, nameof(MinValues));
        Preconditions.AtMost(MinValues ?? 0, MaxOptionCount, nameof(MinValues));
        Preconditions.AtMost(MaxValues ?? 0, MaxOptionCount, nameof(MaxValues));

        Preconditions.AtLeast(Options.Count, MinOptionCount, nameof(Options));
        if (MinValues is not null)
            Preconditions.AtLeast(Options.Count, MinValues.Value, nameof(Options));

        Preconditions.AtMost(Options.Count, MaxOptionCount, nameof(Options));

        return new CheckboxGroupComponent(Id,
            CustomId,
            Options.Select(x => new CheckboxGroupOption(x.Value, x.Label, x.Description, x.DefaultState)).ToImmutableArray(),
            MinValues,
            MaxValues,
            IsRequired);
    }

    /// <inheritdoc/>
    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
