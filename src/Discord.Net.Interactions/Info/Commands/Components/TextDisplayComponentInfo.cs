using Discord.Interactions.Builders;

namespace Discord.Interactions;

public class TextDisplayComponentInfo : ModalComponentInfo
{
    public string Content { get; }

    public TextDisplayComponentInfo(TextDisplayComponentBuilder builder, ModalInfo modal) : base(builder, modal)
    {
        Content = Content;
    }
}
