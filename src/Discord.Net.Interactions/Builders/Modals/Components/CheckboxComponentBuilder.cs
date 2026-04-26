namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating <see cref="CheckboxComponentInfo"/>.
/// </summary>
public class CheckboxComponentBuilder : InputComponentBuilder<CheckboxComponentInfo, CheckboxComponentBuilder>
{
    protected override CheckboxComponentBuilder Instance => this;

    /// <summary>
    ///     Gets or sets whether the checkbox is selected by default.
    /// </summary>
    public bool DefaultState { get; set; } = false;

    internal CheckboxComponentBuilder(ModalBuilder modal) : base(modal) {}

    /// <summary>
    ///     Sets the <see cref="DefaultState"/>
    /// </summary>
    /// <param name="defaultState">Default state of the checkbox.</param>
    /// <returns>The builder instance.</returns>
    public CheckboxComponentBuilder WithDefaultState(bool defaultState)
    {
        DefaultState = defaultState;
        return this;
    }

    internal override CheckboxComponentInfo Build(ModalInfo modal) => new(this, modal);
}
