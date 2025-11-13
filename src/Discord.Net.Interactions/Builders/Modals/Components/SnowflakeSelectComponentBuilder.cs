using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating <see cref="SnowflakeSelectComponentInfo"/>.
/// </summary>
/// <typeparam name="TInfo">The <see cref="SnowflakeSelectComponentInfo"/> this builder yields when built.</typeparam>
/// <typeparam name="TBuilder">Inherited <see cref="SnowflakeSelectComponentBuilder{TInfo, TBuilder}"/> type.</typeparam>
public abstract class SnowflakeSelectComponentBuilder<TInfo, TBuilder> : InputComponentBuilder<TInfo, TBuilder>, ISnowflakeSelectComponentBuilder
    where TInfo : InputComponentInfo
    where TBuilder : InputComponentBuilder<TInfo, TBuilder>, ISnowflakeSelectComponentBuilder
{
    protected readonly List<SelectMenuDefaultValue> _defaultValues;

    /// <inheritdoc/>
    public int MinValues { get; set; } = 1;

    /// <inheritdoc/>
    public int MaxValues { get; set; } = 1;

    /// <inheritdoc/>
    public string Placeholder { get; set; }

    /// <inheritdoc/>
    public IReadOnlyCollection<SelectMenuDefaultValue> DefaultValues => _defaultValues.AsReadOnly();

    /// <inheritdoc/>
    public SelectDefaultValueType? DefaultValuesType
    {
        get
        {
            return ComponentType switch
            {
                ComponentType.UserSelect => SelectDefaultValueType.User,
                ComponentType.RoleSelect => SelectDefaultValueType.Role,
                ComponentType.ChannelSelect => SelectDefaultValueType.Channel,
                ComponentType.MentionableSelect => null,
                _ => throw new InvalidOperationException("Component type must be a snowflake select type."),
            };
        }
    }

    /// <summary>
    ///     Initialize a new <see cref="SnowflakeSelectComponentBuilder{TInfo, TBuilder}"/>.
    /// </summary>
    /// <param name="modal">Parent modal of this input component.</param>
    /// <param name="componentType">Type of this component.</param>
    public SnowflakeSelectComponentBuilder(ModalBuilder modal, ComponentType componentType) : base(modal)
    {
        ValidateComponentType(componentType);

        ComponentType = componentType;
        _defaultValues = new();
    }

    /// <inheritdoc/>
    public TBuilder AddDefaultValue(SelectMenuDefaultValue defaultValue)
    {
        if (DefaultValuesType.HasValue && defaultValue.Type != DefaultValuesType.Value)
            throw new ArgumentException($"Only default values with {Enum.GetName(typeof(SelectDefaultValueType), DefaultValuesType.Value)} are support by {nameof(TInfo)} select type.", nameof(defaultValue));

        _defaultValues.Add(defaultValue);
        return Instance;
    }

    /// <inheritdoc/>
    public override TBuilder WithComponentType(ComponentType componentType)
    {
        ValidateComponentType(componentType);
        return base.WithComponentType(componentType);
    }

    /// <inheritdoc/>
    public TBuilder WithMinValues(int minValues)
    {
        MinValues = minValues;
        return Instance;
    }

    /// <inheritdoc/>
    public TBuilder WithMaxValues(int maxValues)
    {
        MaxValues = maxValues;
        return Instance;
    }

    /// <inheritdoc/>
    public TBuilder WithPlaceholder(string placeholder)
    {
        Placeholder = placeholder;
        return Instance;
    }

    private void ValidateComponentType(ComponentType componentType)
    {
        if (componentType is not (ComponentType.UserSelect or ComponentType.RoleSelect or ComponentType.MentionableSelect or ComponentType.ChannelSelect))
            throw new ArgumentException("Component type must be a snowflake select type.", nameof(componentType));

    }

    /// <inheritdoc/>
    ISnowflakeSelectComponentBuilder ISnowflakeSelectComponentBuilder.AddDefaultValue(SelectMenuDefaultValue defaultValue) => AddDefaultValue(defaultValue);

    /// <inheritdoc/>
    ISnowflakeSelectComponentBuilder ISnowflakeSelectComponentBuilder.WithMinValues(int minValues) => WithMinValues(minValues);

    /// <inheritdoc/>
    ISnowflakeSelectComponentBuilder ISnowflakeSelectComponentBuilder.WithMaxValues(int maxValues) => WithMaxValues(maxValues);

    /// <inheritdoc/>
    ISnowflakeSelectComponentBuilder ISnowflakeSelectComponentBuilder.WithPlaceholder(string placeholder) => WithPlaceholder(placeholder);
}
