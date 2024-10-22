// using System.Diagnostics.CodeAnalysis;
// using Discord.Net.Hanz.Tasks.Actors.Links.V4.SourceTypes;
// using Discord.Net.Hanz.Tasks.Actors.V3;
// using Microsoft.CodeAnalysis;
// using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;
// using LinkSpecificMember = (string Type, string Name, string? Default);
// using LinkSpecificMembers = System.Collections.Generic.List<(string Type, string Name, string? Default)>;
//
//
// namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
//
// public abstract class LinkTypeNode :
//     LinkNode,
//     ITypeImplementerNode
// {
//     public LinkSchematics.Entry Entry { get; }
//
//     public bool RedefinesLinkMembers { get; protected set; }
//
//     public bool WillGenerateImplementation
//         => RootActorNode is not null && Target.Assembly is not LinkActorTargets.AssemblyTarget.Core;
//
//     public string ImplementationClassName
//         => $"__{Target.Assembly}Link{
//             string.Join(
//                 string.Empty,
//                 Parents
//                     .Prepend(this)
//                     .OfType<LinkTypeNode>()
//                     .Select(x => $"{x.Entry.Symbol.Name}{(x.Entry.Symbol.TypeArguments.Length > 0 ? x.Entry.Symbol.TypeArguments.Length : string.Empty)}")
//                     .Reverse()
//             )
//         }";
//
//     public bool IsTemplate => Parent is ActorNode;
//     
//     public Constructor? Constructor { get; private set; }
//
//     public List<Property> Properties { get; } = [];
//
//     public IEnumerable<LinkTypeNode> ParentLinks
//         => Parents.OfType<LinkTypeNode>();
//
//     protected string? ImplementationLinkType => Target.Assembly switch
//     {
//         LinkActorTargets.AssemblyTarget.Rest => FormattedRestLinkType,
//         _ => null
//     };
//
//     public LinkTypeNode? ImplementationBase { get; private set; }
//     public LinkTypeNode? ImplementationChild { get; private set; }
//
//     protected LinkTypeNode(
//         LinkTarget target,
//         LinkSchematics.Entry entry
//     ) : base(target)
//     {
//         Entry = entry;
//
//         // always has a backlink
//         AddChild(new BackLinkNode(Target));
//
//         // if theres extensions
//         LinkExtensionNode.AddTo(Target, this);
//         LinkHierarchyNode.AddTo(Target, this);
//     }
//
//     protected abstract void AddMembers(List<string> members, NodeContext context, Logger logger);
//
//     protected abstract void CreateImplementation(
//         List<string> members,
//         List<string> bases,
//         NodeContext context,
//         Logger logger
//     );
//
//     private void CreateImplementationForNode(
//         List<string> members,
//         HashSet<string> bases,
//         NodeContext context,
//         Logger logger)
//     {
//         if (RootActorNode is null) return;
//
//         var classBases = new List<string>()
//         {
//             FormatAsTypePath()
//         };
//
//         var classMembers = new List<string>();
//
//         if (ImplementationBase is not null)
//         {
//             ImplementationBase.GetPathGenerics(out var baseGenerics, out _);
//
//             classBases.Insert(0,
//                 $"{ImplementationBase.Target.Actor}.{ImplementationClassName}{(
//                     baseGenerics.Count > 0
//                         ? $"<{string.Join(", ", baseGenerics)}>"
//                         : string.Empty
//                 )}"
//             );
//         }
//         else if (RootActorNode.WillGenerateImplementation)
//         {
//             classBases.Insert(0,
//                 $"{Target.Actor}.{RootActorNode.ImplementationClassName}"
//             );
//         }
//
//         CreateImplementation(classMembers, classBases, context, logger);
//
//         foreach (var parentLinkNode in Parents.OfType<LinkTypeNode>())
//         {
//             parentLinkNode.CreateImplementation(classMembers, classBases, context, logger);
//         }
//
//         classMembers.AddRange(Properties.Select(x => x.Format()));
//
//         if (Constructor is not null)
//             classMembers.Add(Constructor.Format());
//
//         GetPathGenerics(out var generics, out var constraints);
//
//         var formattedGenerics = generics.Count > 0 ? $"<{string.Join(", ", generics)}>" : string.Empty;
//         var formattedConstraints = string.Join(Environment.NewLine, constraints);
//
//         RootActorNode.AdditionalTypes.Add(
//             $$"""
//               private protected class {{ImplementationClassName}}{{formattedGenerics}} : 
//                   {{string.Join($",{Environment.NewLine}", classBases.Distinct()).WithNewlinePadding(4)}}{{(
//                       constraints.Count > 0
//                           ? $"{Environment.NewLine}{formattedConstraints}".WithNewlinePadding(4)
//                           : string.Empty
//                   )}}
//               {
//                   {{string.Join(Environment.NewLine, classMembers.Distinct()).WithNewlinePadding(4)}}
//               }
//               """
//         );
//
//         var constructorParamters = Constructor?.GetActualParameters() ?? [];
//
//         members.Add(
//             $$"""
//               internal static new {{FormatAsTypePath()}} Create(
//                   {{(
//                       constructorParamters.Count > 0
//                           ? string.Join(
//                               $",{Environment.NewLine}",
//                               constructorParamters.Select(x =>
//                                   x.Format()
//                               )
//                           ).WithNewlinePadding(4)
//                           : string.Empty
//                   )}}
//               ) => new {{Target.Actor}}.{{ImplementationClassName}}{{formattedGenerics}}({{(
//                   constructorParamters.Count > 0
//                       ? string.Join(
//                           $", ",
//                           constructorParamters.Select(x =>
//                               ToParameterName(x.Name)
//                           )
//                       )
//                       : string.Empty
//               )}});
//               """
//         );
//     }
//
//     private protected override void Visit(NodeContext context, Logger logger)
//     {
//         base.Visit(context, logger);
//         
//         if (Target.Assembly is not LinkActorTargets.AssemblyTarget.Core)
//         {
//             var hasBase = context.TryGetBaseTarget(Target, out var baseTarget);
//             var hasChild = context.TryGetChildTarget(Target, out var childTarget);
//
//             if (hasBase && GetNodeWithEquivalentPathing(baseTarget) is LinkTypeNode
//                 {
//                     WillGenerateImplementation: true
//                 } baseNode)
//                 ImplementationBase = baseNode;
//             else ImplementationBase = null;
//
//             if (hasChild && GetNodeWithEquivalentPathing(childTarget) is LinkTypeNode
//                 {
//                     WillGenerateImplementation: true
//                 } childNode)
//                 ImplementationChild = childNode;
//             else ImplementationChild = null;
//
//             var rootProperties = new List<Property>();
//
//             rootProperties.AddRange(Properties);
//             rootProperties.AddRange(
//                 Parents
//                     .OfType<LinkTypeNode>()
//                     .SelectMany(x => x.Properties)
//             );
//
//             Properties.Clear();
//             Properties.AddRange(
//                 rootProperties.GroupBy(x => x.Name).Select(x => x.First())
//             );
//
//             Constructor = new(
//                 ImplementationClassName,
//                 Properties
//                     .Select(x =>
//                         new ConstructorParamter(
//                             ToParameterName(x.Name),
//                             x.Type,
//                             null,
//                             x
//                         )
//                     )
//                     .ToList(),
//                 RootActorNode?.Constructor
//             );
//         }
//     }
//
//     public override string Build(NodeContext context, Logger logger)
//     {
//         var bases = new HashSet<string>();
//         var members = new List<string>();
//
//         // foreach (var parentLinks in LinkTypesProduct)
//         // {
//         //     bases.Add($"{Target.Actor}.{string.Join(".", parentLinks.Select(x => x.GetTypeName()))}");
//         //     logger.Log($"{path}.{GetTypeName()}: {string.Join(".", parentLinks.Select(x => x.GetTypeName()))}");
//         // }
//
//         switch (Parent)
//         {
//             case ActorNode:
//                 bases.Add($"{Target.Actor}.Link");
//                 bases.Add($"{FormattedLinkType}{FormatRelativeTypePath()}.{GetTypeName()}");
//                 break;
//             default:
//                 bases.UnionWith(SemanticComposition.OfType<LinkTypeNode>().Select(x => x.FormatAsTypePath()));
//                 break;
//         }
//
//         AddMembers(members, context, logger);
//         
//         // foreach (var parentlinkType in Parents.OfType<LinkTypeNode>())
//         // {
//         //     parentlinkType.AddMembers(members, context, logger);
//         // }
//         
//         if (!IsCore)
//         {
//             if (ImplementationLinkType is null) return string.Empty;
//
//             bases.UnionWith([
//                 $"{Target.GetCoreActor()}{FormatRelativeTypePath()}.{GetTypeName()}",
//                 $"{ImplementationLinkType}"
//             ]);
//
//             CreateImplementationForNode(members, bases, context, logger);
//         }
//
//         if (Ancestors.Count > 0)
//         {
//             // redefine get actor
//             // members.AddRange([
//             //     $"new {Target.Actor} GetActor({Target.Id} id);",
//             //     $"{Target.Actor} Discord.IActorProvider<{Target.Actor}, {Target.Id}>.GetActor({Target.Id} id) => GetActor(id);"
//             // ]);
//
//             foreach (var ancestor in Ancestors)
//             {
//                 var ancestorBase = $"{ancestor.Target.Actor}{FormatRelativeTypePath()}.{GetTypeName()}";
//
//                 bases.Add(ancestorBase);
//
//                 // var overrideType = ancestor.GetEntityAssignableAncestors(context).Length > 0
//                 //     ? ancestorBase
//                 //     : $"Discord.IActorProvider<{ancestor.Target.Actor}, {ancestor.Target.Id}>";
//                 //
//                 // members.AddRange([
//                 //     $"{ancestor.Target.Actor} {overrideType}.GetActor({ancestor.Target.Id} id) => GetActor(id);"
//                 // ]);
//             }
//         }
//
//         return
//             $$"""
//               public {{(ShouldDeclareNewType ? "new " : string.Empty)}}interface {{GetTypeName()}} : 
//                   {{string.Join($",{Environment.NewLine}", bases.OrderBy(x => x)).WithNewlinePadding(4)}}{{(
//                       Entry.Syntax.ConstraintClauses.Count > 0
//                           ? $"{Environment.NewLine}{string.Join(Environment.NewLine, Entry.Syntax.ConstraintClauses)}"
//                               .PrefixNewLine()
//                               .WithNewlinePadding(4)
//                           : string.Empty
//                   )}}
//               {
//                   {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
//                   {{BuildChildren(context, logger).WithNewlinePadding(4)}}
//               }
//               """;
//     }
//
//     [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
//     public static bool TryGetNode(LinkTarget target, LinkSchematics.Entry entry, out LinkTypeNode node)
//     {
//         node = entry.Symbol.Name switch
//         {
//             "Indexable" => new IndexableNode(target, entry),
//             "Paged" => new PagedNode(target, entry),
//             "Enumerable" => new EnumerableNode(target, entry),
//             "Defined" => new DefinedNode(target, entry),
//             _ => null!
//         };
//
//         if (node is null) return false;
//
//         foreach (var child in entry.Children)
//         {
//             if (TryGetNode(target, child, out var childNode))
//                 node.AddChild(childNode);
//         }
//
//         return true;
//     }
//
//     public string GetTypeName()
//         => LinksV4.FormatTypeName(Entry.Symbol);
//
//     // LinkSpecificMembers ITypeImplementerNode.RequiredMembers
//     //     => InstanceImplementationMembers;
//     //
//     // LinkSpecificMembers ITypeImplementerNode.ConstructorMembers
//     //     => Parent is ITypeImplementerNode parentImplementer
//     //         ? [..parentImplementer.ConstructorMembers, ..InstanceImplementationMembers]
//     //         : InstanceImplementationMembers;
// }