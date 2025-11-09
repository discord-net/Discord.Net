using Discord.Interactions.Info.InputComponents;
using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders;

public class SelectMenuInputComponentBuilder : InputComponentBuilder<SelectMenuInputComponentInfo, SelectMenuInputComponentBuilder>
{
    private readonly List<SelectMenuOptionBuilder> _options;

    protected override SelectMenuInputComponentBuilder Instance => this;

    public string Placeholder { get; set; }

    public int MinValues { get; set; }

    public int MaxValues { get; set; }

    public IReadOnlyCollection<SelectMenuOptionBuilder> Options => _options;

    public SelectMenuInputComponentBuilder(ModalBuilder modal) : base(modal)
    {
        _options = new();
    }

    public SelectMenuInputComponentBuilder AddOption(SelectMenuOptionBuilder option)
    {
        _options.Add(option);
        return this;
    }

    public SelectMenuInputComponentBuilder AddOption(Action<SelectMenuOptionBuilder> configure)
    {
        var builder = new SelectMenuOptionBuilder();
        configure(builder);
        _options.Add(builder);
        return this;
    }

    internal override SelectMenuInputComponentInfo Build(ModalInfo modal)
        => new(this, modal);
}
