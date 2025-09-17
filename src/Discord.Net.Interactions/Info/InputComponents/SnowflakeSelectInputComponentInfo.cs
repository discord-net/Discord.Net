using Discord.Interactions.Builders.Modals.Inputs;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Interactions.Info.InputComponents;

public abstract class SnowflakeSelectInputComponentInfo : InputComponentInfo
{
    public int MinValues { get; }

    public int MaxValues { get; }

    public string Placeholder { get; }

    public IReadOnlyCollection<SelectMenuDefaultValue> DefaultValues { get; }

    public SelectDefaultValueType? DefaultValueType { get; }

    internal SnowflakeSelectInputComponentInfo(ISnowflakeSelectInputComponentBuilder builder, ModalInfo modal) : base(builder, modal)
    {
        MinValues = builder.MinValues;
        MaxValues = builder.MaxValues;
        Placeholder = builder.Placeholder;
        DefaultValues = builder.DefaultValues.ToImmutableArray();
        DefaultValueType = builder.DefaultValuesType;
    }
}
