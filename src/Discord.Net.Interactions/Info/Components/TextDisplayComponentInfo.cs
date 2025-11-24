using Discord.Interactions.Builders;

namespace Discord.Interactions;

/// <summary>
///     Represents the <see cref="ModalComponentInfo"/> class for <see cref="ComponentType.TextDisplay"/> type.
/// </summary>
public class TextDisplayComponentInfo : ModalComponentInfo
{
    /// <summary>
    ///     Gets the content of the text display.
    /// </summary>
    public string Content { get; }

    internal TextDisplayComponentInfo(TextDisplayComponentBuilder builder, ModalInfo modal) : base(builder, modal)
    {
        Content = Content;
    }
}
