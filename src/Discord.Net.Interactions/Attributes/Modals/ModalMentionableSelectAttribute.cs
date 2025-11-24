namespace Discord.Interactions;

/// <summary>
///     Marks a <see cref="IModal"/> property as a mentionable select input.
/// </summary>
public class ModalMentionableSelectAttribute : ModalSelectComponentAttribute
{
    /// <inheritdoc />
    public override ComponentType ComponentType => ComponentType.MentionableSelect;

    /// <summary>
    ///     Create a new <see cref="ModalMentionableSelectAttribute"/>.
    /// </summary>
    /// <param name="customId">Custom ID of the mentionable select component.</param>
    /// <param name="minValues">Minimum number of values that can be selected.</param>
    /// <param name="maxValues">Maximum number of values that can be selected</param>
    public ModalMentionableSelectAttribute(string customId, int minValues = 1, int maxValues = 1) : base(customId, minValues, maxValues) { }
}
