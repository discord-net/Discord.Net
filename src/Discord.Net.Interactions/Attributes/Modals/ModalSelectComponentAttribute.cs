namespace Discord.Interactions;

/// <summary>
///     Base attribute for select-menu, user, channel, role, and mentionable select inputs in modals.
/// </summary>
public abstract class ModalSelectComponentAttribute : ModalInputAttribute
{
    /// <summary>
    ///     Gets or sets the minimum number of values that can be selected.
    /// </summary>
    public int MinValues { get; set; } = 1;

    /// <summary>
    ///     Gets or sets the maximum number of values that can be selected.
    /// </summary>
    public int MaxValues { get; set; } = 1;

    /// <summary>
    ///     Gets or sets the placeholder text.
    /// </summary>
    public string Placeholder { get; set; }

    internal ModalSelectComponentAttribute(string customId, int minValues = 1, int maxValues = 1) : base(customId)
    {
        MinValues = minValues;
        MaxValues = maxValues;
    }
}
