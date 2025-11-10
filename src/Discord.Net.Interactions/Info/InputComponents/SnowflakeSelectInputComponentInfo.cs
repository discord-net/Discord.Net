using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Interactions;

/// <summary>
///     Represents the base <see cref="InputComponentInfo"/> class for <see cref="ComponentType.UserSelect"/>, <see cref="ComponentType.ChannelSelect"/>, <see cref="ComponentType.RoleSelect"/>, <see cref="ComponentType.MentionableSelect"/> type.
/// </summary>
public abstract class SnowflakeSelectInputComponentInfo : InputComponentInfo
{
    /// <summary>
    ///     Gets the minimum number of values that can be selected.
    /// </summary>
    public int MinValues { get; }

    /// <summary>
    ///     Gets the maximum number of values that can be selected.
    /// </summary>
    public int MaxValues { get; }

    /// <summary>
    ///     Gets the placeholder of this select input.
    /// </summary>
    public string Placeholder { get; }

    /// <summary>
    ///     Gets the default values of this select input.
    /// </summary>
    public IReadOnlyCollection<SelectMenuDefaultValue> DefaultValues { get; }

    /// <summary>
    ///     Gets the default value type of this select input.
    /// </summary>
    public SelectDefaultValueType? DefaultValueType { get; }

    internal SnowflakeSelectInputComponentInfo(Builders.ISnowflakeSelectInputComponentBuilder builder, ModalInfo modal) : base(builder, modal)
    {
        MinValues = builder.MinValues;
        MaxValues = builder.MaxValues;
        Placeholder = builder.Placeholder;
        DefaultValues = builder.DefaultValues.ToImmutableArray();
        DefaultValueType = builder.DefaultValuesType;
    }
}
