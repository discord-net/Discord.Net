using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating <see cref="SelectMenuComponentInfo"/>.
/// </summary>
public class SelectMenuComponentBuilder : InputComponentBuilder<SelectMenuComponentInfo, SelectMenuComponentBuilder>
{
    private readonly List<SelectMenuOptionBuilder> _options;

    protected override SelectMenuComponentBuilder Instance => this;

    /// <summary>
    ///     Gets and sets the placeholder for the select menu iput.
    /// </summary>
    public string Placeholder { get; set; }

    /// <summary>
    ///     Gets and sets the minimum number of values that can be selected.
    /// </summary>
    public int MinValues { get; set; }

    /// <summary>
    ///     Gets or sets the maximum number of values that can be selected.
    /// </summary>
    public int MaxValues { get; set; }

    /// <summary>
    ///     Gets the options of this select menu component.
    /// </summary>
    public IReadOnlyCollection<SelectMenuOptionBuilder> Options => _options;

    /// <summary>
    ///     Initialize a new <see cref="SelectMenuComponentBuilder"/>.
    /// </summary>
    /// <param name="modal">Parent modal of this component.</param>
    public SelectMenuComponentBuilder(ModalBuilder modal) : base(modal)
    {
        _options = new();
    }

    /// <summary>
    ///     Adds an option to <see cref="Options"/>.
    /// </summary>
    /// <param name="option">Option to be added to <see cref="Options"/>.</param>
    /// <returns>The builder instance.</returns>
    public SelectMenuComponentBuilder AddOption(SelectMenuOptionBuilder option)
    {
        _options.Add(option);
        return this;
    }

    /// <summary>
    ///     Adds an option to <see cref="Options"/>.
    /// </summary>
    /// <param name="configure">Select menu option builder factory.</param>
    /// <returns>The builder instance.</returns>
    public SelectMenuComponentBuilder AddOption(Action<SelectMenuOptionBuilder> configure)
    {
        var builder = new SelectMenuOptionBuilder();
        configure(builder);
        _options.Add(builder);
        return this;
    }

    internal override SelectMenuComponentInfo Build(ModalInfo modal)
        => new(this, modal);
}
