// using System.Collections.Immutable;
// using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
// using Discord.Net.Hanz.Tasks.Actors.Links.V4.SourceTypes;
// using Discord.Net.Hanz.Utils;
// using Microsoft.CodeAnalysis;
// using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;
//
// namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;
//
// public class LinkExtensionNode :
//     LinkModifierNode,
//     ITypeImplementerNode
// {
//     public class ExtensionProperty(
//         LinkExtensionNode node,
//         IPropertySymbol symbol,
//         bool isLinkMirror,
//         bool isBackLinkMirror,
//         ActorNode? propertyTarget,
//         LinkExtensionNode? overloadedBase,
//         string? typeHeuristic)
//     {
//         public bool IsOverload => Symbol.ExplicitInterfaceImplementations.Length > 0;
//         public string Name => MemberUtils.GetMemberName(Symbol);
//         public LinkExtensionNode Node { get; } = node;
//         public IPropertySymbol Symbol { get; } = symbol;
//         public LinkExtensionNode? OverloadedBase { get; } = overloadedBase;
//         public bool IsLinkMirror { get; } = isLinkMirror;
//         public bool IsBackLinkMirror { get; } = isBackLinkMirror;
//         public ActorNode? PropertyTarget { get; } = propertyTarget;
//         public bool IsValid { get; private set; }
//         public string Type { get; private set; } = string.Empty;
//
//         public string? TypeHeuristic { get; } = typeHeuristic;
//
//         public bool IsDefinedOnPath => IsValid && (Node.IsAbsoluteTemplate || (!IsBackLinkMirror && IsLinkMirror));
//
//         public ExtensionProperty GetClosestDefinedVariant()
//         {
//             if (
//                 IsDefinedOnPath ||
//                 !Node.InheritedPropertyDefinitions.TryGetValue(Name, out var cartesian)
//             ) return this;
//
//             return cartesian.FirstOrDefault(x => x.IsDefinedOnPath) ?? this;
//         }
//
//         public void Visit(LinkExtensionNode node, Logger logger)
//         {
//             if (IsOverload && OverloadedBase is null)
//             {
//                 logger.Log($"{node}: {Name} has no overload base");
//                 IsValid = false;
//                 return;
//             }
//
//             if (IsBackLinkMirror && PropertyTarget is null)
//             {
//                 logger.Log($"{node}: {Name} has no property target for backlink mirror");
//                 IsValid = false;
//                 return;
//             }
//
//             if (IsLinkMirror && PropertyTarget is null)
//             {
//                 logger.Log($"{node}: {Name} has no property target for link mirror");
//                 IsValid = false;
//                 return;
//             }
//
//             IsValid = true;
//
//             Type = GetPropertyType();
//         }
//
//         public string GetPropertyType(bool backlink = false, bool useCoreTypes = false)
//         {
//             if (!IsValid) return string.Empty;
//
//             var actorType = useCoreTypes
//                 ? PropertyTarget?.Target.GetCoreActor()
//                 : PropertyTarget?.Target.Actor;
//
//             if (IsBackLinkMirror)
//             {
//                 if (backlink)
//                 {
//                     return $"{actorType}.BackLink<TSource>";
//                 }
//
//                 return $"{actorType}";
//             }
//
//             if (IsLinkMirror)
//             {
//                 return Node.IsTemplate
//                     ? (useCoreTypes ? PropertyTarget!.FormattedCoreLink : PropertyTarget!.FormattedLink)
//                     : $"{actorType}{Node.FormatRelativeTypePath(x => x is LinkTypeNode or BackLinkNode)}";
//             }
//
//             return Symbol.Type.ToDisplayString();
//         }
//
//         public override string ToString()
//         {
//             return
//                 $"""
//                  {Name}:
//                  > Valid?: {IsValid}
//                  > Is Overload?: {IsOverload}
//                  > Is BackLink Mirror?: {IsBackLinkMirror}
//                  > Is Link Mirror?: {IsLinkMirror}
//                  > Symbol: {Symbol}
//                  """;
//         }
//     }
//
//     public INamedTypeSymbol ExtensionSymbol { get; }
//
//     public bool IsTemplate => Parents.All(x => x is ActorNode or LinkExtensionNode);
//     public bool IsAbsoluteTemplate => Parent is ActorNode;
//
//     public bool WillGenerateImplementation => !IsCore;
//
//     public string ImplementationClassName => $"__{Target.Assembly}LinkExtension{
//         string.Join(
//             string.Empty,
//             Parents
//                 .Select(x =>
//                     x switch {
//                         LinkTypeNode linkType => $"{linkType.Entry.Symbol.Name}{(linkType.Entry.Symbol.TypeParameters.Length > 0 ? linkType.Entry.Symbol.TypeParameters.Length : string.Empty)}",
//                         LinkHierarchyNode => "Hierarchy",
//                         LinkExtensionNode ext => ext.GetTypeName(),
//                         _ => null
//                     }
//                 )
//                 .OfType<string>()
//                 .Reverse()
//         )
//     }{GetTypeName()}";
//
//     public Constructor? Constructor { get; private set; }
//
//     public List<Property> Properties { get; } = [];
//
//     public List<ExtensionProperty> ExtensionProperties { get; } = [];
//     public HashSet<LinkExtensionNode> ExtendedExtensions { get; } = [];
//
//     public Dictionary<string, ExtensionProperty[]> InheritedPropertyDefinitions { get; private set; } = [];
//
//     public LinkExtensionNode(
//         LinkTarget target,
//         INamedTypeSymbol extensionSymbol
//     ) : base(target)
//     {
//         ExtensionSymbol = extensionSymbol;
//         AddChild(new BackLinkNode(target));
//     }
//
//     private protected override void Visit(NodeContext context, Logger logger)
//     {
//         base.Visit(context, logger);
//
//         ExtendedExtensions.Clear();
//         ExtensionProperties.Clear();
//
//         foreach (var propertySymbol in ExtensionSymbol.GetMembers().OfType<IPropertySymbol>())
//         {
//             var property = GetProperty(propertySymbol, context, logger);
//
//             if (property is not null)
//                 ExtensionProperties.Add(property);
//         }
//
//         foreach (var baseExtension in ExtensionSymbol.Interfaces.Where(IsExtension))
//         {
//             var baseExtensionActor = context.Graph.Nodes
//                 .FirstOrDefault(x => x
//                     .Value
//                     .Target
//                     .GetCoreActor()
//                     .Equals(baseExtension.ContainingType, SymbolEqualityComparer.Default)
//                 )
//                 .Value;
//
//             if (
//                 baseExtensionActor is null ||
//                 GetNodeWithEquivalentPathing(baseExtensionActor) is not LinkExtensionNode extensionNode
//             ) continue;
//
//             ExtendedExtensions.Add(extensionNode);
//         }
//
//         ExtensionProperties.ForEach(x => x.Visit(this, logger));
//
//         Properties.AddRange(
//             ExtensionProperties
//                 .Where(x => x.IsValid && x.Type != string.Empty)
//                 .Select(x =>
//                     new Property(
//                         x.Name,
//                         x.Type,
//                         isVirtual: Children.Any(x => x is BackLinkNode)
//                     )
//                 )
//         );
//
//         Constructor = new(
//             ImplementationClassName,
//             Properties
//                 .Select(x =>
//                     new ConstructorParamter(
//                         ToParameterName(x.Name),
//                         x.Type,
//                         initializes: x
//                     )
//                 )
//                 .ToList(),
//             Parents.OfType<ITypeImplementerNode>().FirstOrDefault(x => x.WillGenerateImplementation)?.Constructor
//         );
//
//         InheritedPropertyDefinitions = SemanticComposition
//             .OfType<LinkExtensionNode>()
//             .SelectMany(x => x.ExtensionProperties)
//             .GroupBy(x => x.Name)
//             .ToDictionary(x => x.Key, x => x.ToArray());
//     }
//
//     private ExtensionProperty? GetProperty(IPropertySymbol symbol, NodeContext context, Logger logger)
//     {
//         LinkExtensionNode? overloadedBase = null;
//
//         if (symbol.ExplicitInterfaceImplementations.Length > 0)
//         {
//             var overloadedProp = symbol.ExplicitInterfaceImplementations[0];
//             var overloadExtensionSymbol = overloadedProp.ContainingType;
//             var overloadActor = overloadedProp.ContainingType.ContainingType;
//
//             var baseExtension =
//                 overloadActor is not null &&
//                 context.Graph.Nodes.TryGetValue(overloadActor, out var node)
//                     ? node
//                     : null;
//
//             if (baseExtension is null)
//             {
//                 logger.Warn(
//                     $"{FormatAsTypePath()}: Failed to find overload extension for '{overloadedProp}' ({overloadActor})");
//                 return null;
//             }
//
//             var baseExtensionNode = Parents.Reverse().Skip(1)
//                 .Aggregate(
//                     (LinkNode?) baseExtension,
//                     (node, part) => node?.Children.FirstOrDefault(x => x.SemanticEquals(part))
//                 );
//
//             if (baseExtensionNode is null)
//             {
//                 logger.Warn(
//                     $"{FormatAsTypePath()}: Failed to find overload extension node for '{overloadedProp}' ({overloadActor})");
//                 return null;
//             }
//
//             if (
//                 baseExtensionNode
//                     .Children
//                     .OfType<LinkExtensionNode>()
//                     .FirstOrDefault(x => x
//                         .ExtensionSymbol
//                         .ToDisplayString()
//                         .Equals(overloadExtensionSymbol.ToDisplayString())
//                     )
//                 is not { } ext
//             )
//             {
//                 logger.Warn(
//                     $"{FormatAsTypePath()}: Failed to find child overload extension '{overloadExtensionSymbol}' node for '{overloadedProp}' ({overloadActor}) ({baseExtensionNode.GetType()} {baseExtensionNode.FormatAsTypePath()})"
//                 );
//
//                 foreach (var child in baseExtensionNode.Children)
//                 {
//                     logger.Log($" - {child.GetType()}: {child.FormatAsTypePath()}");
//                 }
//
//                 return null;
//             }
//
//             overloadedBase = ext;
//             ExtendedExtensions.Add(overloadedBase);
//             // overloadedBase = baseExtension?.Children.OfType<LinkExtensionNode>().FirstOrDefault(
//             //     x => x.ExtensionSymbol.Equals(
//             //         overloadedProp.ContainingType,
//             //         SymbolEqualityComparer.Default
//             //     )
//             // );
//             //
//             // logger.Log(
//             //     $" - Overload search: {overloadedProp.ContainingType} in {overloadedProp.ContainingType.ContainingType}: {overloadedBase}"
//             // );
//             //
//             // if (overloadedBase is not null)
//             //     
//         }
//
//         var linkMirrorAttribute = symbol.GetAttributes()
//             .FirstOrDefault(x => x.AttributeClass?.Name == "LinkMirrorAttribute");
//
//         var typeHeuristicAttribute = symbol.GetAttributes()
//             .FirstOrDefault(x => x.AttributeClass?.Name == "TypeHeuristicAttribute");
//
//         var propertyTarget =
//             symbol.Type is INamedTypeSymbol named
//                 ? IsCore
//                     ? context.Graph.Nodes.TryGetValue(named, out var targetNode)
//                         ? targetNode
//                         : null
//                     : context.Graph.Nodes.FirstOrDefault(x => x
//                         .Value
//                         .Target
//                         .GetCoreActor()
//                         .Equals(named, SymbolEqualityComparer.Default)
//                     ).Value
//                 : null;
//
//         if (
//             symbol.Type.TypeKind is TypeKind.Unknown &&
//             propertyTarget is null)
//         {
//             var propTypeStr = symbol.Type.ToDisplayString();
//             var prefix = Target.Assembly.ToString();
//
//             var actorPart = propTypeStr
//                 .Split(['.'], StringSplitOptions.RemoveEmptyEntries)
//                 .FirstOrDefault(x => x.StartsWith(prefix) && (x.EndsWith("Actor") || x.EndsWith("Trait")));
//
//             if (actorPart is not null)
//             {
//                 var actorSymbol = context.Graph.Compilation
//                     .GetTypeByMetadataName(
//                         $"Discord{(prefix is not "Core" ? $".{prefix}" : string.Empty)}.{actorPart}"
//                     );
//
//                 if (actorSymbol is not null && context.Graph.Nodes.TryGetValue(actorSymbol, out targetNode))
//                     propertyTarget = targetNode;
//             }
//
//             if (
//                 propTypeStr.StartsWith("Indexable") ||
//                 propTypeStr.StartsWith("Enumerable") ||
//                 propTypeStr.StartsWith("Defined") ||
//                 propTypeStr.StartsWith("Paged<")
//             )
//             {
//                 propertyTarget = Parents.OfType<ActorNode>().First();
//             }
//         }
//
//         return new ExtensionProperty(
//             this,
//             symbol,
//             linkMirrorAttribute is not null,
//             linkMirrorAttribute?.NamedArguments
//                 .FirstOrDefault(x => x.Key == "OnlyBackLinks")
//                 .Value.Value as bool? == true,
//             propertyTarget,
//             overloadedBase,
//             typeHeuristicAttribute?.ConstructorArguments.FirstOrDefault().Value as string
//         );
//     }
//
//     private List<string> BuildProperty(ExtensionProperty property)
//     {
//         var results = new List<string>();
//
//         if (property.IsOverload)
//         {
//             results.Add(
//                 $"{property.Type} {property.OverloadedBase!.FormatAsTypePath()}{FormatRelativeTypePath()}.{property.Symbol.ExplicitInterfaceImplementations[0].Name} => {property.Name};");
//             return results;
//         }
//
//         ExtensionProperty[]? existingMembers = null;
//
//         var isNew = !IsCore || InheritedPropertyDefinitions.TryGetValue(property.Name, out existingMembers);
//
//         results.Add($"{(isNew ? "new " : string.Empty)}{property.Type} {property.Name} {{ get; }}");
//
//         var corePath = $"{Target.GetCoreActor()}{FormatRelativeTypePath()}.{GetTypeName()}";
//
//         if (!IsCore)
//         {
//             results.Add(
//                 $"{property.GetPropertyType(useCoreTypes: true)} {corePath}.{property.Name} => {property.Name};"
//             );
//         }
//
//         if (existingMembers?.Length > 0)
//         {
//             results.AddRange(
//                 existingMembers.SelectMany(x =>
//                 {
//                     var result = new List<string>()
//                     {
//                         $"{x.Type} {x.Node.FormatAsTypePath()}.{x.Name} => {x.Name};"
//                     };
//
//                     if (!IsCore)
//                         result.Add(
//                             $"{x.GetPropertyType(useCoreTypes: true)} {corePath}.{x.Name} => {x.Name}");
//
//                     return result;
//                 })
//             );
//         }
//
//         return results;
//     }
//
//     public override string Build(NodeContext context, Logger logger)
//     {
//         var bases = new List<string>();
//         var members = new List<string>();
//
//         members.AddRange(
//             ExtensionProperties
//                 .Where(x => x.IsDefinedOnPath)
//                 .SelectMany(BuildProperty)
//         );
//
//         if (!IsCore)
//         {
//             bases.Add($"{Target.GetCoreActor()}{FormatRelativeTypePath()}.{GetTypeName()}");
//         }
//
//         foreach (var node in ExplicitlyImplements)
//         {
//             logger.Log($"{FormatAsTypePath()} : {node.FormatAsTypePath()}");
//             bases.Add($"{node.FormatAsTypePath()}");
//
//             // if (!IsCore)
//             //     bases.Add($"{Target.GetCoreActor()}{node.FormatRelativeTypePath()}.{GetTypeName()}");
//         }
//
//         foreach (var extendedExtension in ExtendedExtensions)
//         {
//             bases.Add($"{extendedExtension.FormatAsTypePath()}");
//         }
//
//         if (WillGenerateImplementation)
//             CreateImplementation(members, bases, context, logger);
//
//         members.Add(BuildChildren(context, logger));
//
//         return
//             $$"""
//               public {{(ShouldDeclareNewType ? "new " : string.Empty)}}interface {{GetTypeName()}}{{(
//                   bases.Count > 0
//                       ? $" :{Environment.NewLine}{string.Join($",{Environment.NewLine}", bases.OrderBy(x => x))}".WithNewlinePadding(4)
//                       : string.Empty
//               )}}
//               {
//                   {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
//               }
//               """;
//     }
//
//     private void CreateImplementation(
//         List<string> members,
//         List<string> bases,
//         NodeContext context,
//         Logger logger)
//     {
//         if (RootActorNode is null) return;
//
//         var extensionMembers = new List<string>();
//         var extensionBases = new List<string>()
//         {
//             FormatAsTypePath()
//         };
//
//         if (
//             Parents
//                 .OfType<ITypeImplementerNode>()
//                 .FirstOrDefault(x => x.WillGenerateImplementation)
//             is LinkNode baseLinkType and ITypeImplementerNode baseImplementer)
//         {
//             baseLinkType.GetPathGenerics(out var parentGenerics, out _);
//
//             extensionBases.Insert(0, $"{Target.Actor}.{baseImplementer.ImplementationClassName}{(
//                 parentGenerics.Count > 0
//                     ? $"<{string.Join(", ", parentGenerics)}>"
//                     : string.Empty
//             )}");
//         }
//
//         extensionMembers.AddRange(Properties.Select(x => x.Format()));
//
//         extensionMembers.AddRange(
//             ExtensionProperties
//                 .Where(x => Properties.Any(y => y.Name == x.Name))
//                 .Select(x => x.GetClosestDefinedVariant())
//                 .Where(x => x.IsValid)
//                 .Select(x => $"{x.Type} {x.Node.FormatAsTypePath()}.{x.Name} => {x.Name};")
//         );
//
//         if (Constructor is not null)
//             extensionMembers.Add(Constructor.Format());
//
//         GetPathGenerics(out var generics, out var constraints);
//
//         RootActorNode.AdditionalTypes.Add(
//             $$"""
//               private protected class {{ImplementationClassName}}{{(generics.Count > 0 ? $"<{string.Join(", ", generics)}>" : string.Empty)}} : 
//                   {{string.Join($",{Environment.NewLine}", extensionBases.Distinct()).WithNewlinePadding(4)}}
//                   {{string.Join(Environment.NewLine, constraints).WithNewlinePadding(4)}}
//               {
//                   {{string.Join(Environment.NewLine, extensionMembers.Distinct()).WithNewlinePadding(4)}}
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
//     public IEnumerable<string> FormatProperty(ExtensionProperty property, bool backlink = false)
//     {
//         // TODO: overloads
//         if (property.IsOverload) return [];
//
//         //var result = new List<string>();
//
//         if (!property.IsValid)
//         {
//             return [$"// {property.Name} is invalid"];
//         }
//
//         if (property.IsBackLinkMirror)
//         {
//             if (!backlink && IsTemplate)
//             {
//                 return
//                 [
//                     $"{property.PropertyTarget!.Target.Actor} {property.Name} {{ get; }}"
//                 ];
//             }
//
//             if (backlink && IsTemplate)
//             {
//                 return
//                 [
//                     $"new {property.PropertyTarget!.Target.Actor}.BackLink<TSource> {property.Name} {{ get; }}",
//                     $"{property.PropertyTarget!.Target.Actor} {FormatTypePath()}.{GetTypeName()}.{property.Name} => {property.Name};"
//                 ];
//             }
//
//             return
//             [
//                 $"// '{property.Name}' fallthrough for backlink mirror"
//             ];
//         }
//
//         if (property.IsLinkMirror)
//         {
//             if (IsTemplate)
//             {
//                 return [$"{property.PropertyTarget!.FormattedLink} {property.Name} {{ get; }}"];
//             }
//
//             var path = FormatRelativeTypePath(x => x is LinkTypeNode or BackLinkNode);
//
//             if (path == string.Empty)
//                 return
//                 [
//                     $"// '{property.Name}' no path for link mirror"
//                 ];
//
//             return
//             [
//                 $"new {property.PropertyTarget!.Target.Actor}{path} {property.Name} {{ get; }}",
//                 $"{property.PropertyTarget!.FormattedLink} {Target.Actor}.{GetTypeName()}.{property.Name} => {property.Name};"
//             ];
//         }
//
//         if (!IsTemplate || backlink)
//             return
//             [
//                 $"// '{property.Name}' skipped: raw in non-template"
//             ];
//
//         return
//         [
//             $"{property.Symbol.Type} {property.Name} {{ get; }}"
//         ];
//     }
//
//     public string GetTypeName()
//         => ExtensionSymbol.Name.Replace("Extension", string.Empty);
//
//     private static bool IsExtension(INamedTypeSymbol symbol)
//     {
//         return symbol.ContainingType is not null && symbol
//             .GetAttributes()
//             .Any(x => x.AttributeClass?.Name == "LinkExtensionAttribute");
//     }
//
//     public static void AddTo(LinkTarget target, LinkNode node)
//     {
//         var extensionTypes = target.GetCoreActor().GetTypeMembers()
//             .Where(IsExtension)
//             .ToImmutableList();
//
//         if (extensionTypes.Count == 0) return;
//
//         foreach (var extension in extensionTypes)
//         {
//             node.AddChild(Create(extension, extensionTypes));
//         }
//
//         return;
//
//         LinkExtensionNode Create(INamedTypeSymbol extension, ImmutableList<INamedTypeSymbol> children)
//         {
//             var node = new LinkExtensionNode(target, extension);
//
//             var nextChildren = children.Remove(extension);
//
//             foreach (var child in nextChildren)
//             {
//                 node.AddChild(Create(child, nextChildren));
//             }
//
//             return node;
//         }
//     }
//
//     public override string ToString()
//     {
//         return
//             $"""
//              {base.ToString()}
//              Properties: {ExtensionProperties.Count}{(
//                  ExtensionProperties.Count > 0
//                      ? $"{Environment.NewLine}{string.Join(Environment.NewLine, ExtensionProperties.Select(x => $"- {x}"))}"
//                      : string.Empty
//              )}
//              Is Template: {IsTemplate}
//              Symbol: {ExtensionSymbol}
//              """;
//     }
// }