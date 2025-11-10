using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Interactions;

/// <summary>
///     Represents the <see cref="InputComponentInfo"/> class for <see cref="ComponentType.SelectMenu"/> type.
/// </summary>
public class SelectMenuInputComponentInfo : InputComponentInfo
{
    /// <summary>
    ///     Gets the placeholder of the select menu input.
    /// </summary>
    public string Placeholder { get; }

    /// <summary>
    ///     Gets the minimum number of values that can be selected.
    /// </summary>
    public int MinValues { get; }

    /// <summary>
    ///     Gets the maximum number of values that can be selected.
    /// </summary>
    public int MaxValues { get; }

    /// <summary>
    ///     Gets the options of this select menu component.
    /// </summary>
    public IReadOnlyCollection<SelectMenuOption> Options { get; }

    internal SelectMenuInputComponentInfo(Builders.SelectMenuInputComponentBuilder builder, ModalInfo modal) : base(builder, modal)
    {
        Placeholder = builder.Placeholder;
        MinValues = builder.MinValues;
        MaxValues = builder.MaxValues;
        Options = builder.Options.Select(x => x.Build()).ToImmutableArray();
    }
}
