using System;

namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating <see cref="TextDisplayComponentInfo"/>.
/// </summary>
public class TextDisplayComponentBuilder : ModalComponentBuilder<TextDisplayComponentInfo, TextDisplayComponentBuilder>
{
    protected override TextDisplayComponentBuilder Instance => throw new NotImplementedException();

    /// <summary>
    ///     Gets and sets the content of the text display.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    ///     Initialize a new <see cref="TextDisplayComponentBuilder"/>.
    /// </summary>
    /// <param name="modal">Parent modal of this input component.</param>
    public TextDisplayComponentBuilder(ModalBuilder modal) : base(modal)
    {
    }

    /// <summary>
    ///     Sets <see cref="Content"/>.
    /// </summary>
    /// <param name="content">New value of the <see cref="Content"/>.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public TextDisplayComponentBuilder WithContent(string content)
    {
        Content = content;
        return this;
    }

    public override TextDisplayComponentBuilder WithType(Type type)
    {
        if(type != typeof(string))
        {
            throw new ArgumentException($"Text display components can be only used with {typeof(string).Name} properties. {type.Name} provided instead.");
        }

        return base.WithType(type);
    }

    internal override TextDisplayComponentInfo Build(ModalInfo modal)
        => new(this, modal);
}
