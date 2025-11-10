using System.Collections.Generic;

namespace Discord.Interactions.Builders;

/// <summary>
///     Represent a builder for creating <see cref="SnowflakeSelectInputComponentInfo"/>.
/// </summary>
public interface ISnowflakeSelectInputComponentBuilder : IInputComponentBuilder
{
    /// <summary>
    ///     Gets the minimum number of values that can be selected.
    /// </summary>
    int MinValues { get; }

    /// <summary>
    ///     Gets the maximum number of values that can be selected.
    /// </summary>
    int MaxValues { get; }

    /// <summary>
    ///     Gets the placeholder text for this select component.
    /// </summary>
    string Placeholder { get; set; }

    /// <summary>
    ///     Gets the default value collection for this select component.
    /// </summary>
    IReadOnlyCollection<SelectMenuDefaultValue> DefaultValues { get; }

    /// <summary>
    ///     Gets the default value type of this select component.
    /// </summary>
    SelectDefaultValueType? DefaultValuesType { get; }

    /// <summary>
    ///     Adds a default value to the <see cref="DefaultValues"/>.
    /// </summary>
    /// <param name="defaultValue">Default value to be added.</param>
    /// <returns>The builder instance.</returns>
    ISnowflakeSelectInputComponentBuilder AddDefaultValue(SelectMenuDefaultValue defaultValue);

    /// <summary>
    ///     Sets <see cref="MinValues"/>.
    /// </summary>
    /// <param name="minValues">New value of the <see cref="MinValues"/></param>
    /// <returns>The builder instance.</returns>
    ISnowflakeSelectInputComponentBuilder WithMinValues(int minValues);

    /// <summary>
    ///     Sets <see cref="MaxValues"/>.
    /// </summary>
    /// <param name="maxValues">New value of the <see cref="MaxValues"/></param>
    /// <returns>The builder instance.</returns>
    ISnowflakeSelectInputComponentBuilder WithMaxValues(int maxValues);

    /// <summary>
    ///     Sets <see cref="Placeholder"/>.
    /// </summary>
    /// <param name="placeholder">New value of the <see cref="Placeholder"/></param>
    /// <returns>The builder instance.</returns>
    ISnowflakeSelectInputComponentBuilder WithPlaceholder(string placeholder);
}
