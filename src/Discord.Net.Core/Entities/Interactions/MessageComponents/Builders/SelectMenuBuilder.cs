using Discord.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord;

/// <summary>
///     Represents a class used to build <see cref="SelectMenuComponent"/>'s.
/// </summary>
public class SelectMenuBuilder
{
    /// <summary>
    ///     The max length of a <see cref="SelectMenuComponent.Placeholder"/>.
    /// </summary>
    public const int MaxPlaceholderLength = 100;

    /// <summary>
    ///     The maximum number of values for the <see cref="SelectMenuComponent.MinValues"/> and <see cref="SelectMenuComponent.MaxValues"/> properties.
    /// </summary>
    public const int MaxValuesCount = 25;

    /// <summary>
    ///     The maximum number of options a <see cref="SelectMenuComponent"/> can have.
    /// </summary>
    public const int MaxOptionCount = 25;

    /// <summary>
    ///     Gets or sets the custom id of the current select menu.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length exceeds <see cref="ComponentBuilder.MaxCustomIdLength"/></exception>
    /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length subceeds 1.</exception>
    public string CustomId
    {
        get => _customId;
        set => _customId = value?.Length switch
        {
            > ComponentBuilder.MaxCustomIdLength => throw new ArgumentOutOfRangeException(nameof(value), $"Custom Id length must be less or equal to {ComponentBuilder.MaxCustomIdLength}."),
            0 => throw new ArgumentOutOfRangeException(nameof(value), "Custom Id length must be at least 1."),
            _ => value
        };
    }

    /// <summary>
    ///     Gets or sets the type of the current select menu.
    /// </summary>
    /// <exception cref="ArgumentException">Type must be a select menu type.</exception>
    public ComponentType Type
    {
        get => _type;
        set => _type = value.IsSelectType()
            ? value
            : throw new ArgumentException("Type must be a select menu type.", nameof(value));
    }

    /// <summary>
    ///     Gets or sets the placeholder text of the current select menu.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="Placeholder"/> length exceeds <see cref="MaxPlaceholderLength"/>.</exception>
    /// <exception cref="ArgumentException" accessor="set"><see cref="Placeholder"/> length subceeds 1.</exception>
    public string Placeholder
    {
        get => _placeholder;
        set => _placeholder = value?.Length switch
        {
            > MaxPlaceholderLength => throw new ArgumentOutOfRangeException(nameof(value), $"Placeholder length must be less or equal to {MaxPlaceholderLength}."),
            0 => throw new ArgumentOutOfRangeException(nameof(value), "Placeholder length must be at least 1."),
            _ => value
        };
    }

    /// <summary>
    ///     Gets or sets the minimum values of the current select menu.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="MinValues"/> exceeds <see cref="MaxValuesCount"/>.</exception>
    public int MinValues
    {
        get => _minValues;
        set
        {
            Preconditions.AtMost(value, MaxValuesCount, nameof(MinValues));
            _minValues = value;
        }
    }

    /// <summary>
    ///     Gets or sets the maximum values of the current select menu.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="MaxValues"/> exceeds <see cref="MaxValuesCount"/>.</exception>
    public int MaxValues
    {
        get => _maxValues;
        set
        {
            Preconditions.AtMost(value, MaxValuesCount, nameof(MaxValues));
            _maxValues = value;
        }
    }

    /// <summary>
    ///     Gets or sets a collection of <see cref="SelectMenuOptionBuilder"/> for this current select menu.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="Options"/> count exceeds <see cref="MaxOptionCount"/>.</exception>
    /// <exception cref="ArgumentNullException" accessor="set"><see cref="Options"/> is null.</exception>
    public List<SelectMenuOptionBuilder> Options
    {
        get => _options;
        set
        {
            if (value != null)
                Preconditions.AtMost(value.Count, MaxOptionCount, nameof(Options));

            _options = value;
        }
    }

    /// <summary>
    ///     Gets or sets whether the current menu is disabled.
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    ///     Gets or sets the menu's channel types (only valid on <see cref="ComponentType.ChannelSelect"/>s).
    /// </summary>
    public List<ChannelType> ChannelTypes { get; set; }

    public List<SelectMenuDefaultValue> DefaultValues
    {
        get => _defaultValues;
        set
        {
            if (value != null)
                Preconditions.AtMost(value.Count, MaxOptionCount, nameof(DefaultValues));

            _defaultValues = value;
        }
    }

    private List<SelectMenuOptionBuilder> _options = new List<SelectMenuOptionBuilder>();
    private int _minValues = 1;
    private int _maxValues = 1;
    private string _placeholder;
    private string _customId;
    private ComponentType _type = ComponentType.SelectMenu;
    private List<SelectMenuDefaultValue> _defaultValues = new();

    /// <summary>
    ///     Creates a new instance of a <see cref="SelectMenuBuilder"/>.
    /// </summary>
    public SelectMenuBuilder() { }

    /// <summary>
    ///     Creates a new instance of a <see cref="SelectMenuBuilder"/> from instance of <see cref="SelectMenuComponent"/>.
    /// </summary>
    public SelectMenuBuilder(SelectMenuComponent selectMenu)
    {
        Placeholder = selectMenu.Placeholder;
        CustomId = selectMenu.CustomId;
        MaxValues = selectMenu.MaxValues;
        MinValues = selectMenu.MinValues;
        IsDisabled = selectMenu.IsDisabled;
        Options = selectMenu.Options?
           .Select(x => new SelectMenuOptionBuilder(x.Label, x.Value, x.Description, x.Emote, x.IsDefault))
           .ToList();
        DefaultValues = selectMenu.DefaultValues?.ToList();
    }

    /// <summary>
    ///     Creates a new instance of a <see cref="SelectMenuBuilder"/>.
    /// </summary>
    /// <param name="customId">The custom id of this select menu.</param>
    /// <param name="options">The options for this select menu.</param>
    /// <param name="placeholder">The placeholder of this select menu.</param>
    /// <param name="maxValues">The max values of this select menu.</param>
    /// <param name="minValues">The min values of this select menu.</param>
    /// <param name="isDisabled">Disabled this select menu or not.</param>
    /// <param name="type">The <see cref="ComponentType"/> of this select menu.</param>
    /// <param name="channelTypes">The types of channels this menu can select (only valid on <see cref="ComponentType.ChannelSelect"/>s)</param>
    public SelectMenuBuilder(string customId, List<SelectMenuOptionBuilder> options = null, string placeholder = null, int maxValues = 1, int minValues = 1,
        bool isDisabled = false, ComponentType type = ComponentType.SelectMenu, List<ChannelType> channelTypes = null, List<SelectMenuDefaultValue> defaultValues = null)
    {
        CustomId = customId;
        Options = options;
        Placeholder = placeholder;
        IsDisabled = isDisabled;
        MaxValues = maxValues;
        MinValues = minValues;
        Type = type;
        ChannelTypes = channelTypes ?? new();
        DefaultValues = defaultValues ?? new();
    }

    /// <summary>
    ///     Sets the field CustomId.
    /// </summary>
    /// <param name="customId">The value to set the field CustomId to.</param>
    /// <inheritdoc cref="CustomId"/>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuBuilder WithCustomId(string customId)
    {
        CustomId = customId;
        return this;
    }

    /// <summary>
    ///     Sets the field placeholder.
    /// </summary>
    /// <param name="placeholder">The value to set the field placeholder to.</param>
    /// <inheritdoc cref="Placeholder"/>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuBuilder WithPlaceholder(string placeholder)
    {
        Placeholder = placeholder;
        return this;
    }

    /// <summary>
    ///     Sets the field minValues.
    /// </summary>
    /// <param name="minValues">The value to set the field minValues to.</param>
    /// <inheritdoc cref="MinValues"/>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuBuilder WithMinValues(int minValues)
    {
        MinValues = minValues;
        return this;
    }

    /// <summary>
    ///     Sets the field maxValues.
    /// </summary>
    /// <param name="maxValues">The value to set the field maxValues to.</param>
    /// <inheritdoc cref="MaxValues"/>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuBuilder WithMaxValues(int maxValues)
    {
        MaxValues = maxValues;
        return this;
    }

    /// <summary>
    ///     Sets the field options.
    /// </summary>
    /// <param name="options">The value to set the field options to.</param>
    /// <inheritdoc cref="Options"/>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuBuilder WithOptions(List<SelectMenuOptionBuilder> options)
    {
        Options = options;
        return this;
    }

    /// <summary>
    ///     Add one option to menu options.
    /// </summary>
    /// <param name="option">The option builder class containing the option properties.</param>
    /// <exception cref="InvalidOperationException">Options count reached <see cref="MaxOptionCount"/>.</exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuBuilder AddOption(SelectMenuOptionBuilder option)
    {
        Options ??= new();

        if (Options.Count >= MaxOptionCount)
            throw new InvalidOperationException($"Options count reached {MaxOptionCount}.");

        Options.Add(option);
        return this;
    }

    /// <summary>
    ///     Add one option to menu options.
    /// </summary>
    /// <param name="label">The label for this option.</param>
    /// <param name="value">The value of this option.</param>
    /// <param name="description">The description of this option.</param>
    /// <param name="emote">The emote of this option.</param>
    /// <param name="isDefault">Render this option as selected by default or not.</param>
    /// <exception cref="InvalidOperationException">Options count reached <see cref="MaxOptionCount"/>.</exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuBuilder AddOption(string label, string value, string description = null, IEmote emote = null, bool? isDefault = null)
    {
        AddOption(new SelectMenuOptionBuilder(label, value, description, emote, isDefault));
        return this;
    }

    /// <summary>
    ///     Add one default value to menu options.
    /// </summary>
    /// <param name="id">The id of an entity to add.</param>
    /// <param name="type">The type of an entity to add.</param>
    /// <exception cref="InvalidOperationException">Default values count reached <see cref="MaxOptionCount"/>.</exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuBuilder AddDefaultValue(ulong id, SelectDefaultValueType type)
        => AddDefaultValue(new(id, type));

    /// <summary>
    ///     Add one default value to menu options.
    /// </summary>
    /// <param name="value">The default value to add.</param>
    /// <exception cref="InvalidOperationException">Default values count reached <see cref="MaxOptionCount"/>.</exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuBuilder AddDefaultValue(SelectMenuDefaultValue value)
    {
        if (DefaultValues.Count >= MaxOptionCount)
            throw new InvalidOperationException($"Options count reached {MaxOptionCount}.");

        DefaultValues.Add(value);
        return this;
    }

    /// <summary>
    ///     Sets the field default values.
    /// </summary>
    /// <param name="defaultValues">The value to set the field default values to.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuBuilder WithDefaultValues(params SelectMenuDefaultValue[] defaultValues)
    {
        DefaultValues = defaultValues?.ToList();
        return this;
    }

    /// <summary>
    ///     Sets whether the current menu is disabled.
    /// </summary>
    /// <param name="isDisabled">Whether the current menu is disabled or not.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuBuilder WithDisabled(bool isDisabled)
    {
        IsDisabled = isDisabled;
        return this;
    }

    /// <summary>
    ///     Sets the menu's current type.
    /// </summary>
    /// <param name="type">The type of the menu.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuBuilder WithType(ComponentType type)
    {
        Type = type;
        return this;
    }

    /// <summary>
    ///     Sets the menus valid channel types (only for <see cref="ComponentType.ChannelSelect"/>s).
    /// </summary>
    /// <param name="channelTypes">The valid channel types of the menu.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuBuilder WithChannelTypes(List<ChannelType> channelTypes)
    {
        ChannelTypes = channelTypes;
        return this;
    }

    /// <summary>
    ///     Sets the menus valid channel types (only for <see cref="ComponentType.ChannelSelect"/>s).
    /// </summary>
    /// <param name="channelTypes">The valid channel types of the menu.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuBuilder WithChannelTypes(params ChannelType[] channelTypes)
    {
        ChannelTypes = channelTypes is null
            ? ChannelTypeUtils.AllChannelTypes()
            : channelTypes.ToList();
        return this;
    }

    /// <summary>
    ///     Builds a <see cref="SelectMenuComponent"/>
    /// </summary>
    /// <returns>The newly built <see cref="SelectMenuComponent"/></returns>
    public SelectMenuComponent Build()
    {
        var options = Options?.Select(x => x.Build()).ToList();

        return new SelectMenuComponent(CustomId, options, Placeholder, MinValues, MaxValues, IsDisabled, Type, ChannelTypes, DefaultValues);
    }
}
