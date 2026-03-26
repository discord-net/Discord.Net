using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating <see cref="RadioGroupComponentInfo"/>.
/// </summary>
public class RadioGroupComponentBuilder : InputComponentBuilder<RadioGroupComponentInfo, RadioGroupComponentBuilder>
{
    private readonly List<RadioGroupOptionProperties> _options;
    protected override RadioGroupComponentBuilder Instance => this;

    /// <summary>
    ///     Gets the options of this radio group component.
    /// </summary>
    public IReadOnlyCollection<RadioGroupOptionProperties> Options => _options.AsReadOnly();

    internal RadioGroupComponentBuilder(ModalBuilder modal) : base(modal)
    {
        _options = new();
    }

    /// <summary>
    ///     Adds an option to <see cref="Options"/>.
    /// </summary>
    /// <param name="option">Option to be added to <see cref="Options"/>.</param>
    /// <returns>The builder instance.</returns>
    public RadioGroupComponentBuilder AddOption(RadioGroupOptionProperties option)
    {
        _options.Add(option);
        return this;
    }

    /// <summary>
    ///     Adds an option to <see cref="Options"/>.
    /// </summary>
    /// <param name="configure">Radio group option builder factory.</param>
    /// <returns>The builder instance.</returns>
    public RadioGroupComponentBuilder AddOption(Action<RadioGroupOptionProperties> configure)
    {
        var builder = new RadioGroupOptionProperties();
        configure(builder);
        _options.Add(builder);
        return this;
    }

    internal override RadioGroupComponentInfo Build(ModalInfo modal) => new(this, modal);
}
