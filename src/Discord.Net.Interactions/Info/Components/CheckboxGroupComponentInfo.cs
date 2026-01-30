using Discord.Interactions.Builders;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Interactions;

public class CheckboxGroupComponentInfo : InputComponentInfo
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
    ///     Gets the options of this checkbox group component.
    /// </summary>
    public IReadOnlyCollection<CheckboxGroupOption> Options { get; }

    internal CheckboxGroupComponentInfo(CheckboxGroupComponentBuilder builder, ModalInfo modal)
        : base(builder, modal)
    {
        MinValues = builder.MinValues;
        MaxValues = builder.MaxValues;
        Options = builder.Options.Select(x => new CheckboxGroupOption(x.Value, x.Label, x.Description, x.DefaultState)).ToImmutableArray();
    }
}
