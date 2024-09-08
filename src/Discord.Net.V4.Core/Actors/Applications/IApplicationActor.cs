namespace Discord;

public partial interface IApplicationActor :
    IActor<ulong, IApplication>
{
    IApplicationRoleConnectionMetadataActor.Enumerable.BackLink<IApplicationActor> RoleConnectionMetadata { get; }
    IApplicationEmoteActor.Enumerable.Indexable.BackLink<IApplicationActor> Emotes { get; }
}