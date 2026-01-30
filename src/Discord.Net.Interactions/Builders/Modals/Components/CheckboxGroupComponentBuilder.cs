using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders;

public class CheckboxGroupComponentBuilder : InputComponentBuilder<CheckboxGroupComponentInfo, CheckboxGroupComponentBuilder>
{
    private readonly List<CheckboxGroupOptionProperties> _options;
    protected override CheckboxGroupComponentBuilder Instance => this;

    /// <summary>
    ///     Gets the minimum number of values that can be selected.
    /// </summary>
    public int MinValues { get; set; }

    /// <summary>
    ///     Gets the maximum number of values that can be selected.
    /// </summary>
    public int MaxValues { get; set; }

    /// <summary>
    ///     Gets the options of this checkbox group component.
    /// </summary>
    public IReadOnlyCollection<CheckboxGroupOptionProperties> Options => _options.AsReadOnly();

    internal CheckboxGroupComponentBuilder(ModalBuilder modal) : base(modal)
    {
        _options = new();
    }

    /// <summary>
    ///     Adds an option to <see cref="Options"/>.
    /// </summary>
    /// <param name="option">Option to be added to <see cref="Options"/>.</param>
    /// <returns>The builder instance.</returns>
    public CheckboxGroupComponentBuilder AddOption(CheckboxGroupOptionProperties option)
    {
        _options.Add(option);
        return this;
    }

    /// <summary>
    ///     Adds an option to <see cref="Options"/>.
    /// </summary>
    /// <param name="configure">Radio group option builder factory.</param>
    /// <returns>The builder instance.</returns>
    public CheckboxGroupComponentBuilder AddOption(Action<CheckboxGroupOptionProperties> configure)
    {
        var builder = new CheckboxGroupOptionProperties();
        configure(builder);
        _options.Add(builder);
        return this;
    }

    internal override CheckboxGroupComponentInfo Build(ModalInfo modal) => new(this, modal);
}
