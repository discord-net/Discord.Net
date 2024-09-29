using System.Collections.Immutable;

namespace Discord.Net.Hanz.Tasks.Actors.V3.Types;

public class Indexable : ILinkTypeProcessor
{
    public ConstructorRequirements AddImplementation(
        List<string> members,
        LinksV3.Target target, 
        LinkSchematics.Entry type, 
        ImmutableList<LinkSchematics.Entry> path)
    {
        var overrideTarget = target.EntityAssignableAncestors.Count > 0
            ? $"{target.LinkTarget.GetCoreActor()}{LinksV3.FormatPath(path.Add(type))}"
            : $"{target.FormattedCoreLinkType}.Indexable";
        
        members.AddRange([
            $"public {target.LinkTarget.Actor} this[{target.LinkTarget.Id} id] => Provider.GetActor(id);",
            $"public {target.LinkTarget.Actor} Specifically({target.LinkTarget.Id} id) => Provider.GetActor(id);",
            $"{target.LinkTarget.GetCoreActor()} {overrideTarget}.this[{target.LinkTarget.Id} id] => this[id];",
            $"{target.LinkTarget.GetCoreActor()} {overrideTarget}.Specifically({target.LinkTarget.Id} id) => Specifically(id);",
        ]);

        return new ConstructorRequirements();

//         members.Add(
//             $$"""
//               public Indexable(
//                   Discord{{target.LinkTarget.Assembly}}Client client,
//                   {{target.FormattedActorProvider}} provider{{(
//                       target.BaseTarget is not null
//                           ? $"{Environment.NewLine}) : base(client, provider)"
//                           : $"){Environment.NewLine}"
//                   )}}
//               {
//                   Client = client;
//                   Provider = provider;
//               }    
//               """
//         );
    }
    
    public void AddOverrideMembers(
        List<string> members,
        LinksV3.Target target,
        LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path)
    {
        if (target.EntityAssignableAncestors.Count == 0) return;
        
        members.AddRange([
            $"new {target.LinkTarget.Actor} this[{target.LinkTarget.Id} id] => (this as IActorProvider<{target.LinkTarget.Actor}, {target.LinkTarget.Id}>).GetActor(id);",
            $"new {target.LinkTarget.Actor} Specifically({target.LinkTarget.Id} id) => (this as IActorProvider<{target.LinkTarget.Actor}, {target.LinkTarget.Id}>).GetActor(id);"
        ]);

        if (path.Count > 0)
        {
            members.AddRange([
                $"{target.LinkTarget.Actor} {target.LinkTarget.Actor}.{LinksV3.FormatTypeName(type.Symbol)}.this[{target.LinkTarget.Id} id] => this[id];",
                $"{target.LinkTarget.Actor} {target.LinkTarget.Actor}.{LinksV3.FormatTypeName(type.Symbol)}.Specifically({target.LinkTarget.Id} id) => Specifically(id);"
            ]);
        }

        foreach (var ancestor in target.EntityAssignableAncestors)
        {
            var overrideType = ancestor.EntityAssignableAncestors.Count > 0
                ? $"{ancestor.LinkTarget.Actor}{LinksV3.FormatPath(path.Add(type))}"
                : $"{ancestor.FormattedLinkType}.Indexable";

            members.AddRange([
                $"{ancestor.LinkTarget.Actor} {overrideType}.this[{ancestor.LinkTarget.Id} id] => (this as IActorProvider<{target.LinkTarget.Actor}, {target.LinkTarget.Id}>).GetActor(id);",
                $"{ancestor.LinkTarget.Actor} {overrideType}.Specifically({ancestor.LinkTarget.Id} id) => (this as IActorProvider<{target.LinkTarget.Actor}, {target.LinkTarget.Id}>).GetActor(id);"
            ]);
        }
    }
    
    public string? CreateProvider(LinksV3.Target target, Logger logger) => null;
}