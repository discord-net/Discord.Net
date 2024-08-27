namespace Discord;

public interface IApplicationActor :
    IActor<ulong, IApplication>
{
    ApplicationRoleConnectionMetadataLink.Enumerable.BackLink<IApplicationActor> RoleConnectionMetadata { get; }
    ApplicationEmoteLink.Enumerable.Indexable.BackLink<IApplicationActor> Emotes { get; }
}