using Discord.Interactions.Builders;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Interactions;

public class RadioGroupComponentInfo : InputComponentInfo
{
    /// <summary>
    ///     Gets the options of this radio group component.
    /// </summary>
    public IReadOnlyCollection<RadioGroupOption> Options { get; }

    internal RadioGroupComponentInfo(RadioGroupComponentBuilder builder, ModalInfo modal)
        : base(builder, modal)
    {
        Options = builder.Options.Select(x => new RadioGroupOption(x.Value, x.Label, x.Description, x.IsDefault))
            .ToImmutableArray();
    }
}
