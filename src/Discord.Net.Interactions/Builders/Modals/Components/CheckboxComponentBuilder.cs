namespace Discord.Interactions.Builders;

public class CheckboxComponentBuilder : InputComponentBuilder<CheckboxComponentInfo, CheckboxComponentBuilder>
{
    protected override CheckboxComponentBuilder Instance => this;

    public bool DefaultState { get; set; } = false;

    internal CheckboxComponentBuilder(ModalBuilder modal) : base(modal)
    {
    }

    public CheckboxComponentBuilder WithDefaultState(bool defaultState)
    {
        DefaultState = defaultState;
        return this;
    }

    internal override CheckboxComponentInfo Build(ModalInfo modal) => new(this, modal);
}
