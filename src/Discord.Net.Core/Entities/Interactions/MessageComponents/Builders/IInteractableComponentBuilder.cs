namespace Discord;

public interface IInteractableComponentBuilder : IMessageComponentBuilder
{
    string CustomId { get; set; }
}
