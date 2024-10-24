using Discord.Net.Hanz.Tasks.Actors.Links.V4;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Discord.Net.Hanz.Utils.Bakery;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;

public interface IHasActorInfo
{
    ActorInfo ActorInfo { get; }
}

public readonly record struct ActorInfo(
    LinkActorTargets.AssemblyTarget Assembly,
    TypeRef Actor,
    TypeRef Entity,
    TypeRef Id,
    TypeRef Model,
    TypeRef CoreActor,
    TypeRef CoreEntity
)
{
    public bool IsCore => Assembly is LinkActorTargets.AssemblyTarget.Core;

    public string FormattedRelation
        => $"Discord.IRelation<{Id}, {Entity}>";
    
    public string FormattedRelationship
        => $"Discord.IRelationship<{Actor}, {Id}, {Entity}>";
    public string FormattedCanonicalRelationship
        => $"Discord.ICanonicalRelationship<{Actor}, {Id}, {Entity}>";
    
    public string FormattedIdentifiable
        => $"Discord.IIdentifiable<{Id}, {Entity}, {Actor}, {Model}>";

    public string FormattedCoreIdentifiable
        => $"Discord.IIdentifiable<{Id}, {CoreEntity}, {CoreActor}, {Model}>";

    public string FormattedBackLinkType
        => $"Discord.IBackLink<TSource, {Actor}, {Id}, {Entity}, {Model}>";

    public string FormattedCoreBackLinkType
        =>
            $"Discord.IBackLink<TSource, {CoreActor}, {Id}, {CoreEntity}, {Model}>";

    public string FormattedLinkType
        => $"Discord.ILinkType<{Actor}, {Id}, {Entity}, {Model}>";

    public string FormattedCoreLinkType
        => $"Discord.ILinkType<{CoreActor}, {Id}, {CoreEntity}, {Model}>";

    public string FormattedLink
        => $"Discord.ILink<{Actor}, {Id}, {Entity}, {Model}>";

    public string FormattedCoreLink
        => $"Discord.ILink<{CoreActor}, {Id}, {CoreEntity}, {Model}>";

    public string FormattedRestLinkType =>
        $"Discord.Rest.IRestLinkType<{Actor}, {Id}, {Entity}, {Model}>";

    public string FormattedActorProvider
        => $"Discord.IActorProvider<{Actor}, {Id}>";

    public string FormattedCoreActorProvider
        => $"Discord.IActorProvider<{CoreActor}, {Id}>";

    public string FormattedRestActorProvider
        => $"Discord.Rest.RestActorProvider<{Actor}, {Id}>";

    public string FormattedEntityProvider
        => $"Discord.IEntityProvider<{Entity}, {Model}>";

    public string FormattedCoreEntityProvider
        => $"Discord.IEntityProvider<{CoreEntity}, {Model}>";

    public static ActorInfo Create(LinksV5.NodeContext context)
        => Create(context.Target);
    
    public static ActorInfo Create(LinkActorTargets.GenerationTarget target)
    {
        var coreActor = target.Assembly is LinkActorTargets.AssemblyTarget.Core
            ? new TypeRef(target.Actor)
            : new TypeRef(
                Hierarchy.GetHierarchy(target.Actor, false)
                    .First(x =>
                        x.Type.ContainingAssembly.Name == "Discord.Net.V4.Core"
                        &&
                        x.Type.AllInterfaces.Any(y => y is {Name: "IActor", TypeArguments.Length: 2})
                    ).Type
            );

        var coreEntity = target.Assembly is LinkActorTargets.AssemblyTarget.Core
            ? new TypeRef(target.Actor)
            : new TypeRef(
                Hierarchy.GetHierarchy(target.Entity, false)
                    .First(x =>
                        x.Type.ContainingAssembly.Name == "Discord.Net.V4.Core"
                        &&
                        x.Type.AllInterfaces.Any(y => y is {Name: "IEntity"})
                    ).Type
            );

        return new ActorInfo(
            Assembly: target.Assembly,
            Actor: new(target.Actor),
            Entity: new(target.Entity),
            Id: new(target.Id),
            Model: new(target.Model),
            CoreEntity: coreEntity,
            CoreActor: coreActor
        );
    }
}