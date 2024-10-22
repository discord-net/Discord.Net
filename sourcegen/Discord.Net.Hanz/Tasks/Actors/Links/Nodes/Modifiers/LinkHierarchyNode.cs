// using System.Text;
// using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
// using Discord.Net.Hanz.Tasks.Actors.Links.V4.SourceTypes;
// using Discord.Net.Hanz.Tasks.Actors.V3;
// using Microsoft.CodeAnalysis;
// using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;
//
// namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;
//
// using HierarchyProperties = List<(string Type, LinkTarget Target, ActorNode Node)>;
// using HierarchyProperty = (string Type, LinkTarget Target, ActorNode Node);
//
// public class LinkHierarchyNode :
//     LinkModifierNode,
//     ITypeImplementerNode
// {
//     public override bool SupportsAncestralPathing => false;
//
//     public bool IsTemplate => Parent is ActorNode;
//
//     public bool WillGenerateImplementation
//         => RootActorNode is not null && Target.Assembly is not LinkActorTargets.AssemblyTarget.Core;
//
//     public List<ActorNode> HierarchyNodes { get; } = [];
//     public HierarchyProperties HierarchyProperties { get; } = [];
//     public Constructor? Constructor { get; private set; }
//     public List<Property> Properties { get; } = [];
//
//     public HashSet<IPropertySymbol> TypeHeuristicProperties { get; } = new(SymbolEqualityComparer.Default);
//
//     //public HashSet<LinkHierarchyNode> ExplicitlyImplements { get; } = [];
//
//     public string ImplementationClassName
//         => $"__{Target.Assembly}Hierarchy{
//             string.Join(
//                 string.Empty,
//                 Parents
//                     .Select(x =>
//                         x switch {
//                             LinkTypeNode linkType => $"{linkType.Entry.Symbol.Name}{(linkType.Entry.Symbol.TypeParameters.Length > 0 ? linkType.Entry.Symbol.TypeParameters.Length : string.Empty)}",
//                             LinkExtensionNode extension => extension.GetTypeName(),
//                             _ => null
//                         }
//                     )
//                     .OfType<string>()
//                     .Reverse()
//             )
//         }";
//
//     private readonly AttributeData _attribute;
//
//     public LinkHierarchyNode(
//         LinkTarget target,
//         AttributeData attribute
//     ) : base(target)
//     {
//         _attribute = attribute;
//         AddChild(new BackLinkNode(target));
//         LinkExtensionNode.AddTo(Target, this);
//     }
//
//     public string FormatHierarchyNodeName(ActorNode other)
//     {
//         var otherFriendlyName = LinksV4.GetFriendlyName(other.Target.Actor);
//
//         var parts = LinksV4.ToNameParts(otherFriendlyName)
//             .Except(LinksV4.ToNameParts(LinksV4.GetFriendlyName(Target.Actor)))
//             .ToArray();
//
//         if (parts.Length == 0) return otherFriendlyName;
//
//         return string.Join(string.Empty, parts);
//     }
//
//     private protected override void Visit(NodeContext context, Logger logger)
//     {
//         base.Visit(context, logger);
//         
//         HierarchyNodes.Clear();
//         HierarchyProperties.Clear();
//         Properties.Clear();
//         TypeHeuristicProperties.Clear();
//
//         var types = _attribute.NamedArguments
//             .FirstOrDefault(x => x.Key == "Types")
//             .Value;
//
//         var children = types.Kind is not TypedConstantKind.Error
//             ? (
//                 types.Kind switch
//                 {
//                     TypedConstantKind.Array => types.Values.Select(x => (INamedTypeSymbol) x.Value!),
//                     _ => (INamedTypeSymbol[]) types.Value!
//                 }
//             )
//             .Select(x => context.Graph.Nodes.Values
//                 .FirstOrDefault(y =>
//                     y.Target.GetCoreActor().Equals(x, SymbolEqualityComparer.Default)
//                 )
//             )
//             .Where(x => x is not null)
//             .ToArray()
//             : context.Graph.Nodes.Values
//                 .Where(x =>
//                     Hierarchy.Implements(x.Target.GetCoreActor(), Target.GetCoreActor()))
//                 .ToArray();
//
//         logger.Log($"{Target.Actor}: {children.Length} hierarchical link targets");
//
//         HierarchyNodes.AddRange(children);
//
//         HierarchyProperties.AddRange(
//             HierarchyNodes.Select(x =>
//                 (
//                     Type: IsTemplate
//                         ? x.FormattedLink
//                         : FormatTypePath(),
//                     x.Target,
//                     Node: x
//                 )
//             )
//         );
//
//         if (!IsCore)
//         {
//             Properties.AddRange(
//                 HierarchyNodes.Select(x =>
//                     new Property(
//                         FormatHierarchyNodeName(x),
//                         IsTemplate ? x.FormattedLink : $"{x.Target.Actor}{FormatRelativeTypePath()}",
//                         isVirtual: IsTemplate || Children.OfType<BackLinkNode>().Any()
//                     )
//                 )
//             );
//
//             Constructor = new(
//                 ImplementationClassName,
//                 Properties
//                     .Select(x =>
//                         new ConstructorParamter(
//                             ToParameterName(x.Name),
//                             x.Type,
//                             initializes: x
//                         )
//                     )
//                     .ToList(),
//                 (Parent as ITypeImplementerNode)?.Constructor
//             );
//         }
//
//         var entity = Target.GetCoreEntity();
//
//         TypeHeuristicProperties.UnionWith(
//             entity.AllInterfaces.Prepend(entity)
//                 .SelectMany(x => x.GetMembers().OfType<IPropertySymbol>())
//                 .Where(x => x
//                     .GetAttributes()
//                     .Any(v => v.AttributeClass?.ToDisplayString() == "Discord.TypeHeuristicAttribute")
//                 )
//         );
//     }
//
//     public IEnumerable<string> FormatNodesAsOverloads(bool backLink = false, bool useCoreTypes = false)
//     {
//         return HierarchyNodes
//             .Select(x =>
//             {
//                 var name = FormatHierarchyNodeName(x);
//                 var type = IsTemplate
//                     ? backLink
//                         ? (useCoreTypes ? x.FormattedCoreBackLinkType : x.FormattedBackLinkType)
//                         : (useCoreTypes ? x.FormattedCoreLink : x.FormattedLink)
//                     : $"{(useCoreTypes ? x.Target.GetCoreActor() : x.Target.Actor)}{FormatRelativeTypePath()}{(backLink ? ".BackLink<TSource>" : string.Empty)}";
//
//                 return
//                     $"{type} {(useCoreTypes ? Target.GetCoreActor() : Target.Actor)}{FormatRelativeTypePath()}.Hierarchy{(backLink ? ".BackLink<TSource>" : string.Empty)}.{name} => {name};";
//             });
//     }
//
//     public override string Build(NodeContext context, Logger logger)
//     {
//         if (HierarchyNodes.Count == 0) return string.Empty;
//
//         var bases = new List<string>();
//
//         var members = new List<string>(
//             HierarchyNodes.Select(x => FormatHierarchyNodeAsProperty(x))
//         );
//
//         if (!IsCore)
//         {
//             bases.Add($"{Target.GetCoreActor()}{FormatRelativeTypePath()}.Hierarchy");
//             members.AddRange(FormatNodesAsOverloads(useCoreTypes: true));
//         }
//
//         if (IsTemplate)
//         {
//             bases.Add($"{Target.Actor}.Link");    
//         }
//         else
//         {
//             foreach (var baseNode in ExplicitlyImplements)
//             {
//                 bases.Add($"{baseNode.FormatAsTypePath()}");
//             }
//
//             foreach (var relative in SemanticComposition.OfType<LinkHierarchyNode>())
//             {
//                 members.AddRange(relative.FormatNodesAsOverloads());
//                 if (!IsCore)
//                 {
//                     //bases.Add($"{Target.GetCoreActor()}{relative.FormatRelativeTypePath()}.Hierarchy");
//                     members.AddRange(relative.FormatNodesAsOverloads(useCoreTypes: true));
//                 }
//             }
//         }
//
//         if (!IsCore)
//         {
//             CreateImplementation(members, bases);
//         }
//
//         AddDelimiterMethods(members, context, logger);
//
//         members.Add(BuildChildren(context, logger));
//
//         return
//             $$"""
//               public {{(ShouldDeclareNewType ? "new " : string.Empty)}}interface Hierarchy{{(
//                   bases.Count > 0
//                       ? $" :{Environment.NewLine}{string.Join($",{Environment.NewLine}", bases)}".WithNewlinePadding(4)
//                       : string.Empty
//               )}}
//               {
//                   {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
//               }
//               """;
//     }
//
//     private void AddDelimiterMethods(List<string> members, NodeContext context, Logger logger)
//     {
//         logger.Log($"{TypeHeuristicProperties.Count} type heuristic properties on {Target}");
//
//         var overrides = SemanticComposition
//             .OfType<LinkHierarchyNode>()
//             // .Concat(
//             //     GetEntityAssignableAncestors(context)
//             //         .Select(GetNodeWithEquivalentPathing)
//             //         .OfType<LinkHierarchyNode>()
//             // )
//             .ToArray();
//
//         foreach (var property in TypeHeuristicProperties)
//         {
//             if (property.Type.TypeKind is not TypeKind.Enum)
//                 continue;
//
//             var cases = new Dictionary<IFieldSymbol, ActorNode>(SymbolEqualityComparer.Default);
//
//             foreach (var field in property.Type.GetMembers().OfType<IFieldSymbol>())
//             {
//                 var attribute = field
//                     .GetAttributes()
//                     .FirstOrDefault(x => x.AttributeClass?.Name == "TypeHeuristicAttribute");
//
//                 if (attribute?.AttributeClass?.TypeArguments.Length != 1) continue;
//
//                 var childTarget = HierarchyNodes
//                     .FirstOrDefault(x => x
//                         .Target
//                         .GetCoreEntity()
//                         .Equals(attribute.AttributeClass.TypeArguments[0], SymbolEqualityComparer.Default)
//                     );
//
//                 logger.Log($" - {field} -> {(childTarget?.Target.Actor.ToDisplayString() ?? "null")}");
//
//                 if (childTarget is null) continue;
//
//                 cases.Add(field, childTarget);
//             }
//
//             if (cases.Count == 0) continue;
//
//             var propertyOverrides = overrides
//                 .Where(x => x
//                     .TypeHeuristicProperties
//                     .Contains(property)
//                 )
//                 .ToArray();
//             
//             members.AddRange([
//                 $$"""
//                   internal {{(propertyOverrides.Length > 0 ? "new " : string.Empty)}}{{(IsTemplate ? FormattedLink : $"{Target.Actor}{FormatRelativeTypePath()}")}} OfType(
//                       {{property.Type}} delimiter
//                   )
//                   {
//                       return delimiter switch
//                       {
//                           {{
//                               string.Join(
//                                   Environment.NewLine,
//                                   cases.Select(x =>
//                                       $"{x.Key} => {FormatHierarchyNodeName(x.Value)},"
//                                   )
//                               ).WithNewlinePadding(8)
//                           }}
//                           _ => this
//                       };
//                   }
//                   """,
//                 ..propertyOverrides.Select(x =>
//                     $"{(x.IsTemplate ? x.FormattedLink : $"{x.Target.Actor}{x.FormatRelativeTypePath()}")} {x.FormatAsTypePath()}.OfType({property.Type} delimiter) => OfType(delimiter);"
//                 )
//             ]);
//         }
//     }
//
//     public string FormatHierarchyNodeAsProperty(ActorNode node, bool backLink = false)
//     {
//         var type = IsTemplate
//             ? (backLink ? node.FormattedBackLinkType : node.FormattedLink)
//             : $"{node.Target.Actor}{FormatRelativeTypePath()}{(backLink ? ".BackLink<TSource>" : string.Empty)}";
//
//         var name = FormatHierarchyNodeName(node);
//
//         var result = new StringBuilder();
//
//         if (!IsCore || !IsTemplate || backLink)
//             result.Append("new ");
//
//         return result
//             .Append(type)
//             .Append(' ')
//             .Append(name)
//             .Append(" { get; }")
//             .ToString();
//     }
//
//     private void CreateImplementation(
//         List<string> members,
//         List<string> bases)
//     {
//         switch (Target.Assembly)
//         {
//             case LinkActorTargets.AssemblyTarget.Rest:
//                 CreateRestImplementation(members, bases);
//                 break;
//         }
//     }
//
//     private void CreateRestImplementation(
//         List<string> members,
//         List<string> bases)
//     {
//         if (RootActorNode is null) return;
//
//         var hierarchyBases = new List<string>() {FormatAsTypePath()};
//         var hierarchyMembers = new List<string>();
//
//         GetPathGenerics(out var generics, out var constraints);
//
//         if (Parent is ITypeImplementerNode {WillGenerateImplementation: true} implementer)
//         {
//             Parent.GetPathGenerics(out var parentGenerics, out _);
//
//             hierarchyBases.Insert(0, $"{Target.Actor}.{implementer.ImplementationClassName}{(
//                 parentGenerics.Count > 0
//                     ? $"<{string.Join(", ", parentGenerics)}>"
//                     : string.Empty
//             )}");
//         }
//
//         hierarchyMembers.AddRange(
//             Properties.SelectMany(IEnumerable<string> (x) =>
//             [
//                 x.Format(),
//                 $"{x.Type} {FormatAsTypePath()}.{x.Name} => {x.Name};"
//             ])
//         );
//
//         if (Constructor is not null)
//             hierarchyMembers.Add(Constructor.Format());
//
//         var typeName =
//             $"{ImplementationClassName}{(generics.Count > 0 ? $"<{string.Join(", ", generics)}>" : string.Empty)}";
//
//         RootActorNode.AdditionalTypes.Add(
//             $$"""
//               private protected class {{typeName}} : 
//                   {{string.Join($",{Environment.NewLine}", hierarchyBases.Distinct()).WithNewlinePadding(4)}}
//                   {{string.Join(Environment.NewLine, constraints).WithNewlinePadding(4)}}
//               {
//                   {{string.Join(Environment.NewLine, hierarchyMembers).WithNewlinePadding(4)}}
//               }
//               """
//         );
//
//         var ctorParams = Constructor?.GetActualParameters() ?? [];
//
//         members.Add(
//             $$"""
//               internal static new {{FormatAsTypePath()}} Create({{(
//                   ctorParams.Count > 0
//                       ? $"{Environment.NewLine}{string.Join(
//                           $",{Environment.NewLine}",
//                           ctorParams.Select(x =>
//                               x.Format()
//                           )
//                       )}".WithNewlinePadding(4) + Environment.NewLine
//                       : string.Empty
//               )}}) => new {{Target.Actor}}.{{ImplementationClassName}}{{(
//               generics.Count > 0
//                   ? $"<{string.Join(", ", generics)}>"
//                   : string.Empty
//           )}}({{(
//           ctorParams.Count > 0
//               ? string.Join(", ", ctorParams.Select(x => x.Name))
//               : string.Empty
//       )}});
//               """
//         );
//     }
//
//     public string GetTypeName()
//         => "Hierarchy";
//
//     public static void AddTo(LinkTarget target, LinkNode node)
//     {
//         var hierarchyAttribute =
//             target.GetCoreActor()
//                 .GetAttributes()
//                 .FirstOrDefault(x => x.AttributeClass?.Name == "LinkHierarchicalRootAttribute");
//
//         if (hierarchyAttribute is null) return;
//
//         node.AddChild(new LinkHierarchyNode(target, hierarchyAttribute));
//     }
//
//     public override string ToString()
//     {
//         return
//             $"""
//              {base.ToString()}
//              Types: {HierarchyProperties.Count}{(
//                  HierarchyProperties.Count > 0
//                      ? $"{Environment.NewLine}{string.Join(Environment.NewLine, HierarchyProperties.Select(x => $"- {x.Type} ({x.Target.Actor})"))}"
//                      : string.Empty
//              )}
//              Is Template: {IsTemplate}
//              """;
//     }
// }