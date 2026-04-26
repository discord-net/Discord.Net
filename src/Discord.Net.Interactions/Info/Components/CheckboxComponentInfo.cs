using Discord.Interactions.Builders;

namespace Discord.Interactions;

public class CheckboxComponentInfo : InputComponentInfo
{
    /// <summary>
    ///     Gets the default state of this checkbox.
    /// </summary>
    public bool DefaultState { get; }

    internal CheckboxComponentInfo(CheckboxComponentBuilder builder, ModalInfo modal)
        : base(builder, modal)
    {
        DefaultState = builder.DefaultState;
    }
}
