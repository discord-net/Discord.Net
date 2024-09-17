namespace Discord;

public partial interface IInteractionActor :
    IActor<ulong, IInteraction>
{   
    IInteractionWithIdAndTokenActor this[string token] { get; }

    public interface ClientLink : Indexable
    {
        IInteractionWithTokenActor this[string token] { get; }
    }
}