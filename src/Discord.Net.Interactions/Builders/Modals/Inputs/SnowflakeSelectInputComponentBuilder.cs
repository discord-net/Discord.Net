using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders.Modals.Inputs;

public abstract class SnowflakeSelectInputComponentBuilder<TInfo, TBuilder> : InputComponentBuilder<TInfo, TBuilder>, ISnowflakeSelectInputComponentBuilder
    where TInfo : InputComponentInfo
    where TBuilder : InputComponentBuilder<TInfo, TBuilder>, ISnowflakeSelectInputComponentBuilder
{
    protected readonly List<SelectMenuDefaultValue> _defaultValues;

    public int MinValues { get; set; } = 1;

    public int MaxValues { get; set; } = 1;

    public string Placeholder { get; set; }

    public IReadOnlyCollection<SelectMenuDefaultValue> DefaultValues => _defaultValues.AsReadOnly();

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

    public SnowflakeSelectInputComponentBuilder(ModalBuilder modal, ComponentType componentType) : base(modal)
    {
        ValidateComponentType(componentType);

        ComponentType = componentType;
        _defaultValues = new();
    }

    public TBuilder AddDefaultValue(SelectMenuDefaultValue defaultValue)
    {
        if (DefaultValuesType.HasValue && defaultValue.Type != DefaultValuesType.Value)
            throw new ArgumentException($"Only default values with {Enum.GetName(typeof(SelectDefaultValueType), DefaultValuesType.Value)} are support by {nameof(TInfo)} select type.", nameof(defaultValue));

        _defaultValues.Add(defaultValue);
        return Instance;
    }

    public override TBuilder WithComponentType(ComponentType componentType)
    {
        ValidateComponentType(componentType);
        return base.WithComponentType(componentType);
    }

    public TBuilder WithMinValues(int minValues)
    {
        MinValues = minValues;
        return Instance;
    }

    public TBuilder WithMaxValues(int maxValues)
    {
        MaxValues = maxValues;
        return Instance;
    }

    public TBuilder WithPlaceholder(string placeholder)
    {
        Placeholder = placeholder;
        return Instance;
    }

    private void ValidateComponentType(ComponentType componentType)
    {
        if (componentType is not ComponentType.UserSelect or ComponentType.RoleSelect or ComponentType.MentionableSelect or ComponentType.ChannelSelect)
            throw new ArgumentException("Component type must be a snowflake select type.", nameof(componentType));

    }

    ISnowflakeSelectInputComponentBuilder ISnowflakeSelectInputComponentBuilder.AddDefaultValue(SelectMenuDefaultValue defaultValue) => AddDefaultValue(defaultValue);

    ISnowflakeSelectInputComponentBuilder ISnowflakeSelectInputComponentBuilder.WithMinValues(int minValues) => WithMinValues(minValues);

    ISnowflakeSelectInputComponentBuilder ISnowflakeSelectInputComponentBuilder.WithMaxValues(int maxValues) => WithMaxValues(maxValues);

    ISnowflakeSelectInputComponentBuilder ISnowflakeSelectInputComponentBuilder.WithPlaceholder(string placeholder) => WithPlaceholder(placeholder);
}
