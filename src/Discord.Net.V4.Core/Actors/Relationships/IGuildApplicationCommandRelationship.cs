namespace Discord;

public interface IGuildApplicationCommandRelationship :
    IRelationship<IGuildApplicationCommandActor, ulong, IGuildApplicationCommand>
{
    IGuildApplicationCommandActor Command { get; }

    IGuildApplicationCommandActor
        IRelationship<IGuildApplicationCommandActor, ulong, IGuildApplicationCommand>.
        RelationshipActor => Command;
}