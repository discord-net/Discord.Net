namespace Discord;

/// <summary>
///     Represents a builder for an interactable component.
/// </summary>
public interface IInteractableComponentBuilder : IMessageComponentBuilder
{
    /// <summary>
    ///     Gets or sets the custom id for the component.
    /// </summary>
    string CustomId { get; set; }
}
