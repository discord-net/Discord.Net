namespace Discord;

public interface IApplicationRelationship : 
    IRelationship<IApplicationActor, ulong, IApplication>
{
    IApplicationActor Application { get; }

    IApplicationActor IRelationship<IApplicationActor, ulong, IApplication>.RelationshipActor => Application;
}