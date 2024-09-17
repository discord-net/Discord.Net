namespace Discord;

public partial interface IApplicationActor :
    IActor<ulong, IApplication>
{
    IGuildApplicationActor.Indexable Guilds { get; }
    
    IApplicationRoleConnectionMetadataActor.Enumerable.BackLink<IApplicationActor> RoleConnectionMetadata { get; }
    
    IApplicationEmoteActor.Enumerable.Indexable.BackLink<IApplicationActor> Emotes { get; }
    
    ISkuActor.Enumerable Skus { get; }

    [LinkExtension]
    private interface WithCurrentExtension
    {
        ICurrentApplicationActor Current { get; }
    }
}