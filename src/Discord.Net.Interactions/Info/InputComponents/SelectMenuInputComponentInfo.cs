using Discord.Interactions.Builders;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Interactions.Info.InputComponents;
public class SelectMenuInputComponentInfo : InputComponentInfo
{
    public string Placeholder { get; set; }

    public int MinValues { get; set; }

    public int MaxValues { get; set; }

    public IReadOnlyCollection<SelectMenuOption> Options { get; }

    internal SelectMenuInputComponentInfo(SelectMenuInputComponentBuilder builder, ModalInfo modal) : base(builder, modal)
    {
        Placeholder = builder.Placeholder;
        MinValues = builder.MinValues;
        MaxValues = builder.MaxValues;
        Options = builder.Options.Select(x => x.Build()).ToImmutableArray();
    }
}
