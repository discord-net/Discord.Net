namespace Discord.Interactions;

/// <summary>
///     Marks a <see cref="IModal"/> property as a text input.
/// </summary>
public class ModalTextDisplayAttribute : ModalComponentAttribute
{
    /// <inheritdoc/>
    public override ComponentType ComponentType => ComponentType.TextDisplay;

    /// <summary>
    ///     Gets the content of the text display.
    /// </summary>
    public string Content { get; }

    /// <summary>
    ///     Create a new <see cref="ModalTextInputAttribute"/>.
    /// </summary>
    /// <param name="content">Content of the text display.</param>
    /// <param name="id">Optional identifier for component.</param>
    public ModalTextDisplayAttribute(string content = null, int? id = null)
        : base(id)
    {
        Content = content;
    }
}
