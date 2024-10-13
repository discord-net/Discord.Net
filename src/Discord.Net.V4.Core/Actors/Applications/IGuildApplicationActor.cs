namespace Discord;

public partial interface IGuildApplicationActor :
    IApplicationActor,
    IGuildActor.CanonicalRelationship
{
    IGuildApplicationCommandActor.Enumerable.Indexable Commands { get; }
}