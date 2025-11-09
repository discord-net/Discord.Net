using System.Collections.Generic;

namespace Discord.Interactions.Builders.Modals.Inputs;

public interface ISnowflakeSelectInputComponentBuilder : IInputComponentBuilder
{
    int MinValues { get; }

    int MaxValues { get; }

    string Placeholder { get; set; }

    IReadOnlyCollection<SelectMenuDefaultValue> DefaultValues { get; }

    SelectDefaultValueType? DefaultValuesType { get; }

    ISnowflakeSelectInputComponentBuilder AddDefaultValue(SelectMenuDefaultValue defaultValue);

    ISnowflakeSelectInputComponentBuilder WithMinValues(int minValues);

    ISnowflakeSelectInputComponentBuilder WithMaxValues(int maxValues);

    ISnowflakeSelectInputComponentBuilder WithPlaceholder(string placeholder);
}
