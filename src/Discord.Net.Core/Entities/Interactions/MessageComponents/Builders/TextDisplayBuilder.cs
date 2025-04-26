using System;

namespace Discord;

public class TextDisplayBuilder : IMessageComponentBuilder
{
    /// <summary>
    ///     The maximum length of the content.
    /// </summary>
    public const int MaxContentLength = 4096;

    /// <inheritdoc/>
    public ComponentType Type => ComponentType.ActionRow;

    /// <inheritdoc/>
    public int? Id { get; set; }

    /// <summary>
    ///     Gets or sets the content of the text display.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    ///     Initializes a new <see cref="TextDisplayBuilder"/>.
    /// </summary>
    public TextDisplayBuilder() { }

    /// <summary>
    ///     Initializes a new <see cref="TextDisplayBuilder"/> with the specified content.
    /// </summary>
    public TextDisplayBuilder(string content, int? id = null)
    {
        Content = content;
        Id = id;
    }

    /// <summary>
    ///     Sets the content of the text display.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public TextDisplayBuilder WithContent(string content)
    {
        Content = content;
        return this;
    }

    /// <inheritdoc cref="IMessageComponentBuilder.Build"/>
    public TextDisplayComponent Build()
    {
        if (Content.Length > MaxContentLength)
            throw new ArgumentException($"Content length must be less than or equal to {MaxContentLength}.", nameof(Content));

        return new(Content, Id);
    }

    /// <inheritdoc/>
    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
