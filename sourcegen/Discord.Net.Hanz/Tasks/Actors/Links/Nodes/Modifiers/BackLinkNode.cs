// using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
// using Discord.Net.Hanz.Tasks.Actors.Links.V4.SourceTypes;
// using Discord.Net.Hanz.Tasks.Actors.V3;
// using Discord.Net.Hanz.Utils;
// using Microsoft.CodeAnalysis;
// using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;
//
// namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;
//
// public class BackLinkNode(LinkTarget target) :
//     LinkModifierNode(target),
//     ITypeImplementerNode
// {
//     public bool IsTemplate
//         => Parent is ActorNode;
//
//     public bool WillGenerateImplementation
//         => RootActorNode is not null && Target.Assembly is not LinkActorTargets.AssemblyTarget.Core;
//
//     public bool IsClass => !IsCore && IsTemplate;
//
//     public bool HasImplementation { get; private set; }
//     //public bool RedefinesSource { get; private set; }
//
//     public string ImplementationClassName
//         => $"__{Target.Assembly}{
//             string.Join(
//                 string.Empty,
//                 Parents
//                     .Select(x =>
//                         x switch {
//                             LinkTypeNode linkType => $"{linkType.Entry.Symbol.Name}{(linkType.Entry.Symbol.TypeParameters.Length > 0 ? linkType.Entry.Symbol.TypeParameters.Length : string.Empty)}",
//                             LinkExtensionNode extension => extension.GetTypeName(),
//                             LinkHierarchyNode => "Hierarchy",
//                             _ => null
//                         }
//                     )
//                     .OfType<string>()
//                     .Reverse()
//             )
//         }BackLink";
//
//     public Constructor? Constructor { get; private set; }
//
//     public List<Property> Properties { get; } = [];
//
//     public List<BackLinkNode> InheritedBackLinks { get; } = [];
//
//     public bool IsCovariantSource
//         => IsCore ||
//            (
//                !IsClass &&
//                Parents
//                    .OfType<LinkExtensionNode>()
//                    .All(v => v
//                        .ExtensionProperties
//                        .All(x => !x.IsBackLinkMirror)
//                    )
//            );
//
//     private protected override void Visit(NodeContext context, Logger logger)
//     {
//         base.Visit(context, logger);
//         
//         InheritedBackLinks.Clear();
//         Properties.Clear();
//
//         HasImplementation = Target.Assembly is not LinkActorTargets.AssemblyTarget.Core;
//         //RedefinesSource = HasImplementation || GetEntityAssignableAncestors(context).Length > 0;
//
//         // foreach (var relative in ContainingNodes.Select(x => x.Children.OfType<BackLinkNode>().FirstOrDefault()))
//         // {
//         //     if (relative is null || relative == this) continue;
//         //     
//         //     InheritedBackLinks.Add(relative);
//         //     RedefinesSource |= relative.RedefinesSource;
//         // }
//
//         //ConstructorMembers.AddRange(RequiredMembers);
//         Properties.Add(new("Source", "TSource"));
//
//
//         if (!IsCore)
//         {
//             switch (Parent)
//             {
//                 case LinkHierarchyNode hierarchy:
//                     Properties.AddRange(
//                         hierarchy.HierarchyNodes
//                             .Select(x =>
//                                 new Property(
//                                     hierarchy.FormatHierarchyNodeName(x),
//                                     hierarchy.IsTemplate
//                                         ? x.FormattedBackLinkType
//                                         : $"{x.Target.Actor}{hierarchy.FormatRelativeTypePath()}.BackLink<TSource>",
//                                     isOverride: true
//
//                                     // x.Name,
//                                     // hierarchy.IsTemplate  ? x.$"{x.Type}.BackLink<TSource>",
//                                     // x.HasSetter,
//                                     // isOverride: true
//                                 )
//                             )
//                     );
//                     break;
//                 case LinkExtensionNode extension:
//                     Properties.AddRange(
//                         extension.ExtensionProperties
//                             .Where(x => x.IsLinkMirror || x.IsBackLinkMirror)
//                             .Select(x =>
//                                 new Property(
//                                     x.Name,
//                                     x.GetPropertyType(true),
//                                     isOverride: true
//                                 )
//                             )
//                     );
//                     break;
//             }
//         }
//
//         Constructor = new(
//             ImplementationClassName,
//             Properties
//                 .Select(x =>
//                     new ConstructorParamter(
//                         ToParameterName(x.Name),
//                         x.Type,
//                         null,
//                         x)
//                 )
//                 .ToList(),
//             Parents
//                 .OfType<ITypeImplementerNode>()
//                 .FirstOrDefault(x => x.WillGenerateImplementation)
//                 ?.Constructor
//         );
//     }
//
//     private void CreateBackLinkInterface(
//         HashSet<string> members,
//         List<string> bases,
//         NodeContext context,
//         Logger logger)
//     {
//         if (!IsCore)
//         {
//             bases.AddRange([
//                 FormattedBackLinkType,
//                 $"{Target.GetCoreActor()}{FormatRelativeTypePath()}.BackLink<TSource>"
//             ]);
//         }
//
//         if (!IsTemplate)
//         {
//             foreach (var baseNode in ExplicitlyImplements)
//             {
//                 if (baseNode is BackLinkNode {IsTemplate: true}) continue;
//
//                 bases.Add(baseNode.FormatAsTypePath());
//                 logger.Log($"{FormatAsTypePath()} += {baseNode.FormatAsTypePath()}");
//
//                 // switch (baseNode)
//                 // {
//                 //     case LinkHierarchyNode hierarchy:
//                 //         members.UnionWith(hierarchy.FormatNodesAsOverloads());
//                 //         // if(!IsCore)
//                 //         //     members.UnionWith(hierarchy.FormatNodesAsOverloads(useCoreTypes: true));
//                 //         break;
//                 //     case BackLinkNode { Parent: LinkHierarchyNode hierarchy}:
//                 //         members.UnionWith(hierarchy.FormatNodesAsOverloads(backLink: true));
//                 //         // if(!IsCore)
//                 //         //     members.UnionWith(hierarchy.FormatNodesAsOverloads(backLink: true, useCoreTypes: true));
//                 //         break;
//                 // }
//             }
//         }
//
//         // if (RedefinesSource)
//         // {
//         //     members.UnionWith([
//         //         "new TSource Source { get; }",
//         //         $"TSource {FormattedCoreBackLinkType}.Source => Source;"
//         //     ]);
//         //
//         //     if (!IsCore)
//         //         members.Add($"TSource {FormattedBackLinkType}.Source => Source;");
//         // }
//
//         foreach (var ancestor in Ancestors)
//         {
//             if (GetNodeWithEquivalentPathing(ancestor) is not BackLinkNode {PathSupportsAncestralPathing: true} ancestorBackLink) continue;
//
//             bases.Add(ancestorBackLink.FormatAsTypePath());
//
//             // var overrideType = ancestorBackLink.RedefinesSource
//             //     ? $"{ancestorPath}.BackLink<TSource>"
//             //     : ancestor.FormattedBackLinkType;
//
//             //members.Add($"TSource {overrideType}.Source => Source;");
//         }
//
//         switch (Parent)
//         {
//             case LinkHierarchyNode hierarchy:
//                 // members.UnionWith(hierarchy.HierarchyNodes.Select(x => 
//                 //     $"new {}"
//                 // ));
//
//                 members.UnionWith(hierarchy.HierarchyNodes.Select(x =>
//                     hierarchy.FormatHierarchyNodeAsProperty(x, true))
//                 );
//
//                 //members.UnionWith(hierarchy.FormatNodesAsOverloads());
//
//                 members.UnionWith(
//                     hierarchy
//                         .SemanticComposition
//                         .OfType<LinkHierarchyNode>()
//                         .Prepend(hierarchy)
//                         .SelectMany(x =>
//                         {
//                             var result = x
//                                 .FormatNodesAsOverloads();
//
//                             if (x != hierarchy)
//                             {
//                                 result = result.Concat(
//                                     x.FormatNodesAsOverloads(backLink: true)
//                                 );
//                             }
//
//                             if (!IsCore)
//                                 result = result
//                                     .Concat(x.FormatNodesAsOverloads(useCoreTypes: true))
//                                     .Concat(x.FormatNodesAsOverloads(useCoreTypes: true, backLink: true));
//
//                             return result;
//                         })
//                 );
//
//                 // members.UnionWith(hierarchy
//                 //     .HierarchyNodes
//                 //     .SelectMany(IEnumerable<string> (x) =>
//                 //     {
//                 //         var type = hierarchy.IsTemplate
//                 //             ? x.FormattedBackLinkType
//                 //             : $"{x.Target.Actor}{hierarchy.FormatRelativeTypePath()}.BackLink<TSource>";
//                 //
//                 //         var name = hierarchy.FormatHierarchyNodeName(x);
//                 //
//                 //         var overrideType = hierarchy.IsTemplate
//                 //             ? x.FormattedLink
//                 //             : $"{x.Target.Actor}{hierarchy.FormatRelativeTypePath()}";
//                 //
//                 //         var results = new List<string>();
//                 //
//                 //         results.Add($"new {type} {name} {{ get;}}");
//                 //         //results.Add($"{overrideType} {hierarchy.FormatAsTypePath()}.{name} => {name};");
//                 //
//                 //         hierarchy.
//                 //         //
//                 //         // if (!IsCore)
//                 //         // {
//                 //         //     var coreType = hierarchy.IsTemplate
//                 //         //         ? x.FormattedCoreLink
//                 //         //         : $"{x.Target.GetCoreActor()}{hierarchy.FormatRelativeTypePath()}";
//                 //         //
//                 //         //     var coreOverrideType = hierarchy.IsTemplate
//                 //         //         ? x.FormattedCoreBackLinkType
//                 //         //         : $"{x.Target.GetCoreActor()}{hierarchy.FormatRelativeTypePath()}.BackLink<TSource>";
//                 //         //
//                 //         //     results.AddRange([
//                 //         //         $"{coreOverrideType} {Target.GetCoreActor()}{hierarchy.FormatRelativeTypePath()}.Hierarchy.BackLink<TSource>.{name} => {name};",
//                 //         //         $"{coreType} {Target.GetCoreActor()}{hierarchy.FormatRelativeTypePath()}.Hierarchy.{name} => {name};",
//                 //         //     ]);
//                 //         // }
//                 //
//                 //         return results;
//                 //     })
//                 // );
//
//                 break;
//             case LinkExtensionNode extension:
//                 members.UnionWith(
//                     extension.ExtensionProperties
//                         .Where(x =>
//                             x.IsValid &&
//                             x.IsLinkMirror &&
//                             (
//                                 x.IsDefinedOnPath ||
//                                 (x.IsBackLinkMirror && extension.IsTemplate)
//                             )
//                         )
//                         .SelectMany(x =>
//                         {
//                             var result = new List<string>()
//                             {
//                                 $"new {x.GetPropertyType(backlink: true)} {x.Name} {{ get; }}"
//                             };
//
//                             if (x.IsDefinedOnPath)
//                                 result.Add($"{x.Type} {extension.FormatAsTypePath()}.{x.Name} => {x.Name};");
//
//                             if (!IsCore)
//                             {
//                                 var overrideTarget = $"{Target.GetCoreActor()}{FormatRelativeTypePath()}";
//                                 result.AddRange([
//                                     $"{x.GetPropertyType(false, true)} {overrideTarget}.{x.Name} => {x.Name};",
//                                     $"{x.GetPropertyType(true, true)} {overrideTarget}.BackLink<TSource>.{x.Name} => {x.Name};"
//                                 ]);
//                             }
//
//                             if (extension.InheritedPropertyDefinitions.TryGetValue(x.Name, out var cartesian))
//                             {
//                                 result.AddRange(cartesian
//                                     .Where(x => x.IsDefinedOnPath)
//                                     .SelectMany(x =>
//                                     {
//                                         var result = new List<string>()
//                                         {
//                                             $"{x.Type} {x.Node.FormatAsTypePath()}.{x.Name} => {x.Name};",
//                                             $"{x.GetPropertyType(backlink: true)} {x.Node.FormatAsTypePath()}.BackLink<TSource>.{x.Name} => {x.Name};",
//                                         };
//
//                                         if (!IsCore)
//                                         {
//                                             var overrideType =
//                                                 $"{Target.GetCoreActor()}{x.Node.FormatRelativeTypePath()}.{x.Node.GetTypeName()}";
//
//                                             result.AddRange([
//                                                 $"{x.GetPropertyType(useCoreTypes: true)} {overrideType}.{x.Name} => {x.Name};",
//                                                 $"{x.GetPropertyType(useCoreTypes: true, backlink: true)} {overrideType}.BackLink<TSource>.{x.Name} => {x.Name};",
//                                             ]);
//                                         }
//
//                                         return result;
//                                     })
//                                 );
//                             }
//
//                             return result;
//                         })
//                 );
//                 break;
//         }
//     }
//
//     public override string Build(NodeContext context, Logger logger)
//     {
//         var members = new HashSet<string>();
//
//         var bases = new List<string>()
//         {
//             FormatTypePath(),
//             FormattedBackLinkType
//         };
//
//         if (IsClass)
//             CreateImplementation(members, bases, context, logger);
//         else
//         {
//             CreateBackLinkInterface(members, bases, context, logger);
//
//             if (!IsCore)
//                 CreateImplementation(members, bases, context, logger);
//         }
//
//         var kind = IsClass ? "class" : "interface";
//         var name = IsClass ? GetTypeName() : $"BackLink<{(IsCovariantSource ? "out " : string.Empty)}TSource>";
//
//         return
//             $$"""
//               public {{(ShouldDeclareNewType ? "new " : string.Empty)}}{{kind}} {{name}} :
//                   {{string.Join($",{Environment.NewLine}", bases.Distinct()).WithNewlinePadding(4)}}
//                   where TSource : class, IPathable
//               {
//                   {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
//               }
//               """;
//     }
//
//     private void CreateImplementation(
//         HashSet<string> members,
//         List<string> bases,
//         NodeContext context,
//         Logger logger)
//     {
//         if (IsClass)
//         {
//             CreateRootImplementation(members, bases, context, logger);
//             return;
//         }
//
//         if (RootActorNode is null) return;
//
//         // var ancestors = GetAncestors(context);
//         var backLinkMembers = new List<string>()
//         {
//             $"TSource {FormattedBackLinkSourceContainer}.Source => Source;"
//         };
//
//         backLinkMembers.AddRange(Properties.Select(x => x.Format()));
//
//         switch (Parent)
//         {
//             case LinkHierarchyNode hierarchy:
//                 backLinkMembers.AddRange(
//                     hierarchy.HierarchyNodes.SelectMany(IEnumerable<string> (x) =>
//                     {
//                         var type = hierarchy.IsTemplate
//                             ? x.FormattedBackLinkType
//                             : $"{x.Target.Actor}{hierarchy.FormatRelativeTypePath()}.BackLink<TSource>";
//
//                         var overloadType = hierarchy.IsTemplate
//                             ? x.FormattedLink
//                             : $"{x.Target.Actor}{hierarchy.FormatRelativeTypePath()}";
//
//                         var name = hierarchy.FormatHierarchyNodeName(x);
//
//                         return
//                         [
//                             $"{type} {FormatAsTypePath()}.{name} => {name};",
//                             $"{overloadType} {Target.Actor}{FormatRelativeTypePath()}.{name} => {name};"
//                         ];
//                     })
//                 );
//                 break;
//             case LinkExtensionNode extension:
//                 backLinkMembers.AddRange(
//                     extension.ExtensionProperties
//                         .Where(x => x.IsLinkMirror || x.IsBackLinkMirror)
//                         .Select(x => x.GetClosestDefinedVariant())
//                         .Where(x => x.IsValid)
//                         .Select(x =>
//                             $"{x.GetPropertyType(true)} {x.Node.FormatAsTypePath()}.BackLink<TSource>.{x.Name} => {x.Name};"
//                         )
//                 );
//                 break;
//         }
//
//         var backlinkBases = new List<string>()
//         {
//             FormatAsTypePath()
//         };
//
//         if (Parent is ITypeImplementerNode {WillGenerateImplementation: true} implementer)
//         {
//             Parent.GetPathGenerics(out var parentGenerics, out _);
//
//             backlinkBases.Insert(0, $"{Target.Actor}.{implementer.ImplementationClassName}{(
//                 parentGenerics.Count > 0
//                     ? $"<{string.Join(", ", parentGenerics)}>"
//                     : string.Empty
//             )}");
//         }
//         else if (RootActorNode.WillGenerateImplementation)
//         {
//             backlinkBases.Insert(0, $"{Target.Actor}.{RootActorNode.ImplementationClassName}");
//         }
//
//         GetPathGenerics(out var generics, out var constraints);
//
//         if (Constructor is not null)
//             backLinkMembers.Add(Constructor.Format());
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
//               )}}) => new {{Target.Actor}}.{{ImplementationClassName}}<{{string.Join(", ", generics)}}>({{(
//               ctorParams.Count > 0
//                   ? string.Join(", ", ctorParams.Select(x => x.Name))
//                   : string.Empty
//           )}});
//               """
//         );
//
//         RootActorNode.AdditionalTypes.Add(
//             $$"""
//               private sealed class {{ImplementationClassName}}<{{string.Join(", ", generics)}}> : 
//                   {{string.Join($",{Environment.NewLine}", backlinkBases.Distinct()).WithNewlinePadding(4)}}
//                   {{string.Join(Environment.NewLine, constraints).WithNewlinePadding(4)}}
//               {
//                   {{string.Join(Environment.NewLine, backLinkMembers.Distinct()).WithNewlinePadding(4)}}
//               }   
//               """
//         );
//     }
//
//     private void CreateRootImplementation(
//         HashSet<string> members,
//         List<string> bases,
//         NodeContext context,
//         Logger logger)
//     {
//         // create the root back link type
//         bases.AddRange([
//             FormattedBackLinkType,
//             $"{Target.GetCoreActor()}.BackLink<TSource>",
//             $"{Target.Actor}.Link"
//         ]);
//
//         // only need to define once, since we'll
//         members.UnionWith([
//             "internal TSource Source { get; }",
//             $"TSource {FormattedBackLinkSourceContainer}.Source => Source;"
//         ]);
//
//         BuildConstructorsFromTargetActor(members);
//
//         members.UnionWith([
//             $"{Target.Actor} {Target.Actor}.Link.GetActor({Target.Id} id) => this;",
//             $"{Target.Entity} {Target.Actor}.Link.CreateEntity({Target.Model} model) => CreateEntity(model);",
//         ]);
//
//         switch (Target.Assembly)
//         {
//             case LinkActorTargets.AssemblyTarget.Rest:
//                 members.Add($"{FormattedActorProvider} {FormattedRestLinkType}.Provider => this;");
//                 break;
//         }
//     }
//
//     private void BuildConstructorsFromTargetActor(HashSet<string> members, string? typeName = null)
//     {
//         typeName ??= "BackLink";
//
//         foreach (var constructor in Target.Actor.InstanceConstructors)
//         {
//             var parameters = constructor
//                 .Parameters
//                 .Select(x =>
//                     $"{x.Type} {x.Name}{(x.HasExplicitDefaultValue ? $" = {SyntaxUtils.CreateLiteral(x.Type, x.ExplicitDefaultValue)}" : string.Empty)}"
//                 )
//                 .Prepend("TSource source");
//
//             var args = constructor
//                 .Parameters
//                 .Select(x => x.Name);
//
//             members.Add(
//                 $$"""
//                   internal {{typeName}}(
//                       {{string.Join($",{Environment.NewLine}", parameters).WithNewlinePadding(4)}}
//                   ) : base(
//                       {{string.Join($",{Environment.NewLine}", args).WithNewlinePadding(4)}}
//                   )
//                   {
//                       Source = source;
//                   }
//                   """
//             );
//         }
//     }
//
//     public string GetTypeName()
//         => "BackLink<TSource>";
//
//     public override string ToString()
//     {
//         return
//             $"""
//              {base.ToString()}
//              Is Template?: {IsTemplate}
//              Has Implementation?: {HasImplementation}
//              """;
//
// //        {(
// //                  InheritedBackLinks.Count > 0
// //                      ? $"{Environment.NewLine}{string.Join(Environment.NewLine, InheritedBackLinks.Select(x => $"- {x}".Replace(Environment.NewLine, $"{Environment.NewLine}  > ")))}"
// //                      : string.Empty
// //              )}
//     }
// }