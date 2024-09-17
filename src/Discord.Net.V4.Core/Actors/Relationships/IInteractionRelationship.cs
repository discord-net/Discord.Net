namespace Discord;

public interface IInteractionRelationship : 
    IRelationship<IInteractionActor, ulong, IInteraction>
{
    IInteractionActor Interaction { get; }

    IInteractionActor IRelationship<IInteractionActor, ulong, IInteraction>.RelationshipActor => Interaction;
}