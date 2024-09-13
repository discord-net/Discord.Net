namespace Discord;

public partial interface IGuildApplicationActor :
    IApplicationActor,
    IGuildRelationship
{
    IGuildApplicationCommandActor.Enumerable.Indexable Commands { get; }
}