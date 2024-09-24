using System.Collections.Immutable;
using System.Text;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors;

public static class LinkHierarcicalRoots
{
    private static void ApplyGetMethod<T>(
        ref T syntax,
        LinksV2.GenerationTarget target,
        LinksV2.GenerationTarget[] children,
        Logger logger,
        SourceProductionContext context)
        where T : TypeDeclarationSyntax
    {
        var entity = target.GetCoreEntity();

        var typeHeuristicProperties = entity.AllInterfaces.Prepend(entity)
            .SelectMany(x => x.GetMembers().OfType<IPropertySymbol>())
            .Where(x => x
                .GetAttributes()
                .Any(v => v.AttributeClass?.ToDisplayString() == "Discord.TypeHeuristicAttribute")
            );

        foreach (var property in typeHeuristicProperties)
        {
            if (property.Type.TypeKind is not TypeKind.Enum)
                continue;

            var cases = new Dictionary<IFieldSymbol, LinksV2.GenerationTarget>(SymbolEqualityComparer.Default);

            foreach (var field in property.Type.GetMembers().OfType<IFieldSymbol>())
            {
                var attribute = field
                    .GetAttributes()
                    .FirstOrDefault(x => x.AttributeClass?.Name == "TypeHeuristicAttribute");

                if (attribute?.AttributeClass?.TypeArguments.Length != 1) continue;

                logger.Log($" - {field} -> {attribute.AttributeClass.TypeArguments[0]}");

                var childTarget = children
                    .FirstOrDefault(x => x
                        .GetCoreEntity()
                        .Equals(attribute.AttributeClass.TypeArguments[0], SymbolEqualityComparer.Default)
                    );

                if (childTarget is null) continue;

                cases.Add(field, childTarget);
            }

            if (cases.Count == 0) continue;

            logger.Log($"{target.Actor}: {cases.Count} cases for {property}");

            foreach (var item in cases)
            {
                logger.Log($" - {item.Key} -> {item.Value.Actor}");
            }

            var methods = syntax.DescendantNodes()
                .OfType<TypeDeclarationSyntax>()
                .Where(x => x.Identifier.ValueText == "Hierarchy")
                .Select(x =>
                {
                    var path = x.Ancestors()
                        .OfType<TypeDeclarationSyntax>()
                        .TakeWhile(x => x.Identifier.ValueText != target.Actor.Name)
                        .Reverse()
                        .ToArray();

                    if (path.Length == 0) return null;

                    var pathStr = string.Join(".", path.Select(LinksV2.ToReferenceName));

                    var generics = string.Join(
                        ", ",
                        path.SelectMany(x => x.TypeParameterList?.Parameters ?? [])
                            .Select(x => x.Identifier)
                    );

                    var constraints = string.Join(
                        Environment.NewLine,
                        path.SelectMany(x => x.ConstraintClauses)
                    ).WithNewlinePadding(4);

                    if (!generics.Equals(string.Empty))
                        generics = $"<{generics}>";

                    if (!constraints.Equals(string.Empty))
                        constraints = $"{Environment.NewLine}    {constraints}";
                        
                        
                    return
                        $$"""
                          public static {{target.Actor}}.{{pathStr}} OfType{{generics}}(
                              this {{target.Actor}}.{{pathStr}}.Hierarchy hierarchy,
                              {{property.Type}} type
                          ){{constraints}}
                          {
                              switch (type)
                              {
                                  {{
                                      string.Join(
                                          Environment.NewLine,
                                          cases.Select(x =>
                                              $"""
                                               case {x.Key}:
                                                   return hierarchy.{
                                                       string.Join(
                                                           string.Empty,
                                                           LinksV2.ToNameParts(LinksV2.GetFriendlyName(x.Value.Actor))
                                                               .Except(LinksV2.ToNameParts(LinksV2.GetFriendlyName(target.Actor)))
                                                       )
                                                   };
                                               """
                                          )
                                      )
                                      .WithNewlinePadding(8)
                                  }}
                                  default: return hierarchy;
                              }
                          }
                          """;
                })
                .Where(x => x is not null);

            context.AddSource(
                $"LinkExtensions/{target.Actor.ToFullMetadataName()}Hierarchy",
                $$"""
                  using Discord;

                  namespace {{target.Actor.ContainingNamespace}};

                  public static class {{target.Actor.Name}}HierarchyExtensions
                  {
                      {{string.Join(Environment.NewLine, methods).WithNewlinePadding(4)}}
                  }
                  """
            );
        }
    }

    public static void ApplyHierarchicalRoot<T>(
        ref T syntax,
        LinksV2.GenerationTarget target,
        ImmutableArray<LinksV2.GenerationTarget?> targets,
        Logger logger,
        SourceProductionContext context
    )
        where T : TypeDeclarationSyntax
    {
        var hierarchyAttribute =
            target.GetCoreActor()
                .GetAttributes()
                .FirstOrDefault(x => x.AttributeClass?.Name == "LinkHierarchicalRootAttribute");

        if (hierarchyAttribute is null) return;

        var types = hierarchyAttribute.NamedArguments
            .FirstOrDefault(x => x.Key == "Types")
            .Value;

        if (types.Kind is TypedConstantKind.Array)
        {
            foreach (var t in types.Values)
            {
                logger.Log($"- {t.Kind} : {t.Value?.GetType()}");
            }
        }

        var children = types.Kind is not TypedConstantKind.Error
            ? (
                types.Kind switch
                {
                    TypedConstantKind.Array => types.Values.Select(x => (INamedTypeSymbol) x.Value!),
                    _ => (INamedTypeSymbol[]) types.Value!
                }
            )
            .Select(x => targets
                .FirstOrDefault(y =>
                    y is not null
                    &&
                    y.GetCoreActor().Equals(x, SymbolEqualityComparer.Default)
                )
            )
            .OfType<LinksV2.GenerationTarget>()
            .ToArray()
            : targets
                .Where(x => x is not null && Hierarchy.Implements(x.GetCoreActor(), target.GetCoreActor()))
                .OfType<LinksV2.GenerationTarget>()
                .ToArray();

        logger.Log($"{target.Actor}: {children.Length} hierarchical link targets");

        if (children.Length == 0) return;

        foreach (var child in children)
        {
            logger.Log($" - {child.Actor}");
        }

        if (target.Assembly is LinksV2.AssemblyTarget.Core)
        {
            syntax = (T) syntax.AddMembers(
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      public interface Hierarchy
                      {
                          {{
                              string.Join(
                                  Environment.NewLine,
                                  children
                                      .Select(x =>
                                      {
                                          var name = string.Join(
                                              string.Empty,
                                              LinksV2.ToNameParts(LinksV2.GetFriendlyName(x.Actor))
                                                  .Except(LinksV2.ToNameParts(LinksV2.GetFriendlyName(target.Actor)))
                                          );

                                          return SyntaxFactory.ParseMemberDeclaration(
                                              $"Discord.ILink<{x.Actor}, {x.Id}, {x.Entity}, {x.Model}> {name} {{ get; }}"
                                          )!;
                                      })
                              )
                          }}
                      }  
                      """
                )!
            );
        }

        syntax = syntax
            .ReplaceNodes(
                syntax.DescendantNodes().OfType<T>(),
                (old, node) =>
                {
                    if (node.Identifier.ValueText == "Hierarchy")
                        return node;

                    if (node.Identifier.ValueText is "BackLink") return node;

                    var anscestors = old.AncestorsAndSelf()
                        .OfType<T>()
                        .ToList();

                    var path = string.Join(
                        ".",
                        anscestors
                            .TakeWhile(x => x.Identifier.ValueText != target.Actor.Name)
                            .Select(x =>
                                $"{x.Identifier}{
                                    (x.TypeParameterList?.Parameters.Count > 0
                                        ? $"{x.TypeParameterList.WithParameters(
                                            SyntaxFactory.SeparatedList(
                                                x.TypeParameterList.Parameters
                                                    .Select(x => x
                                                        .WithVarianceKeyword(default)
                                                    )
                                            )
                                        )}"
                                        : string.Empty)
                                }"
                            )
                            .Reverse()
                    );

                    logger.Log($"{target.Actor} += {children.Length} children to {path}");
                    foreach (var ancestor in anscestors)
                    {
                        logger.Log($" - {ancestor.Identifier}");
                    }

                    var props = children.Select(x =>
                    {
                        var name = string.Join(
                            string.Empty,
                            LinksV2.ToNameParts(LinksV2.GetFriendlyName(x.Actor))
                                .Except(LinksV2.ToNameParts(LinksV2.GetFriendlyName(target.Actor)))
                        );

                        var result = new StringBuilder()
                            .AppendLine(
                                old is ClassDeclarationSyntax
                                    ? $"public virtual {x.Actor}.{path} {name} {{ get; }}"
                                    : $"new {x.Actor}.{path} {name} {{ get; }}"
                            );

                        if (target.Assembly is not LinksV2.AssemblyTarget.Core && anscestors.Count >= 1)
                        {
                            result.AppendLine(
                                $"{x.GetCoreActor()}.{path} {target.GetCoreActor()}.{path}.Hierarchy.{name} => {name};"
                            );
                        }
                        else
                        {
                            result.AppendLine(
                                $"Discord.ILink<{x.Actor}, {x.Id}, {x.Entity}, {x.Model}> {(target.Assembly is LinksV2.AssemblyTarget.Core ? target.Actor : target.GetCoreActor())}.Hierarchy.{name} => {name};"
                            );
                        }

                        return result.ToString();
                    });

                    var hierarchy = (T) SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          public {{(old is ClassDeclarationSyntax ? "class" : "interface")}} Hierarchy : {{path}}
                          {
                              {{
                                  string.Join(
                                      Environment.NewLine,
                                      props
                                  )
                              }}
                          }
                          """
                    )!;

                    hierarchy = (T) hierarchy
                        .AddBaseListTypes(
                            SyntaxFactory.SimpleBaseType(
                                SyntaxFactory.ParseTypeName(
                                    target.Assembly is LinksV2.AssemblyTarget.Core
                                        ? $"{target.Actor}.Hierarchy"
                                        : $"{target.GetCoreActor()}.{path}.Hierarchy"
                                )
                            )
                        );

                    var ctors = old.Members
                        .OfType<ConstructorDeclarationSyntax>()
                        .Select(x => x.ParameterList)
                        .Prepend(old.ParameterList)
                        .ToArray();

                    if (old is ClassDeclarationSyntax)
                    {
                        foreach
                        (
                            var ctor
                            in ctors
                        )
                        {
                            if (ctor is null) continue;

                            hierarchy = (T) hierarchy.AddMembers(
                                SyntaxFactory.ParseMemberDeclaration(
                                    $$"""
                                      public Hierarchy{{ctor
                                          .AddParameters(children
                                              .Select(x => {
                                                  var name = string.Join(
                                                      string.Empty,
                                                      LinksV2.ToNameParts(LinksV2.GetFriendlyName(x.Actor))
                                                          .Except(LinksV2.ToNameParts(LinksV2.GetFriendlyName(target.Actor)))
                                                  );

                                                  name = $"{char.ToLower(name[0])}{name.Substring(1)}";

                                                  return SyntaxFactory.Parameter(
                                                      [],
                                                      [],
                                                      SyntaxFactory.ParseTypeName(
                                                          $"{x.Actor}.{path}"
                                                      ),
                                                      SyntaxFactory.Identifier(name),
                                                      null
                                                  );
                                              })
                                              .ToArray()
                                          )
                                          .NormalizeWhitespace()
                                      }} : base({{string.Join(", ", ctor.Parameters.Select(x => x.Identifier))}})
                                      {
                                         {{
                                             string.Join(
                                                 Environment.NewLine,
                                                 children.Select(x => {
                                                     var name = string.Join(
                                                         string.Empty,
                                                         LinksV2.ToNameParts(LinksV2.GetFriendlyName(x.Actor))
                                                             .Except(LinksV2.ToNameParts(LinksV2.GetFriendlyName(target.Actor)))
                                                     );

                                                     return $"{name} = {char.ToLower(name[0])}{name.Substring(1)};";
                                                 })
                                             )
                                         }}
                                      }
                                      """
                                )!
                            );
                        }
                    }

                    LinksV2.AddBackLink(ref hierarchy, target, logger, false, false, transformer: backlink =>
                    {
                        backlink = (T) backlink
                            .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                            .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                            .WithSemicolonToken(default)
                            .AddMembers(
                                children.SelectMany(x =>
                                {
                                    var name = string.Join(
                                        string.Empty,
                                        LinksV2.ToNameParts(LinksV2.GetFriendlyName(x.Actor))
                                            .Except(LinksV2.ToNameParts(LinksV2.GetFriendlyName(target.Actor)))
                                    );

                                    if (target.Assembly is LinksV2.AssemblyTarget.Core)
                                    {
                                        return (MemberDeclarationSyntax[])
                                        [
                                            SyntaxFactory.ParseMemberDeclaration(
                                                $"new {x.Actor}.{path}.BackLink<TSource> {name} {{ get; }}"
                                            )!,
                                            SyntaxFactory.ParseMemberDeclaration(
                                                $"{x.Actor}.{path} {target.Actor}.{path}.Hierarchy.{name} => {name};"
                                            )!
                                        ];
                                    }

                                    return (MemberDeclarationSyntax[])
                                    [
                                        SyntaxFactory.ParseMemberDeclaration(
                                            $"public override {x.Actor}.{path}.BackLink<TSource> {name} {{ get; }}"
                                        )!,
                                        SyntaxFactory.ParseMemberDeclaration(
                                            $"{x.GetCoreActor()}.{path}.BackLink<TSource> {target.GetCoreActor()}.{path}.Hierarchy.BackLink<TSource>.{name} => {name};"
                                        )!
                                    ];
                                }).ToArray()
                            );

                        if (target.Assembly is LinksV2.AssemblyTarget.Core) return backlink;

                        backlink = backlink
                            .RemoveNodes(
                                backlink.Members.OfType<ConstructorDeclarationSyntax>(),
                                SyntaxRemoveOptions.KeepNoTrivia
                            )!;

                        foreach (var ctor in ctors)
                        {
                            if (ctor is null) continue;

                            backlink = (T) backlink
                                .AddMembers(
                                    SyntaxFactory.ParseMemberDeclaration(
                                        $$"""
                                          public BackLink{{ctor
                                              .WithParameters(
                                                  SyntaxFactory.SeparatedList([
                                                      SyntaxFactory.Parameter(
                                                          [],
                                                          [],
                                                          SyntaxFactory.ParseTypeName("TSource"),
                                                          SyntaxFactory.Identifier("source"),
                                                          null
                                                      ),
                                                      ..ctor.Parameters,
                                                      ..children
                                                          .Select(x => {
                                                              var name = string.Join(
                                                                  string.Empty,
                                                                  LinksV2.ToNameParts(LinksV2.GetFriendlyName(x.Actor))
                                                                      .Except(LinksV2.ToNameParts(LinksV2.GetFriendlyName(target.Actor)))
                                                              );

                                                              name = $"{char.ToLower(name[0])}{name.Substring(1)}";

                                                              return SyntaxFactory.Parameter(
                                                                  [],
                                                                  [],
                                                                  SyntaxFactory.ParseTypeName(
                                                                      $"{x.Actor}.{path}.BackLink<TSource>"
                                                                  ),
                                                                  SyntaxFactory.Identifier(name),
                                                                  null
                                                              );
                                                          })
                                                  ])
                                              )
                                              .NormalizeWhitespace()
                                          }} : base({{
                                              string.Join(
                                                  ", ",
                                                  ctor.Parameters
                                                      .Select(x => x.Identifier.ValueText)
                                                      .Concat(children.Select(x => {
                                                          var name = string.Join(
                                                              string.Empty,
                                                              LinksV2.ToNameParts(LinksV2.GetFriendlyName(x.Actor))
                                                                  .Except(LinksV2.ToNameParts(LinksV2.GetFriendlyName(target.Actor)))
                                                          );

                                                          return $"{char.ToLower(name[0])}{name.Substring(1)}";
                                                      }))
                                              )
                                          }})
                                          {
                                              Source = source;
                                             {{
                                                 string.Join(
                                                     Environment.NewLine,
                                                     children.Select(x => {
                                                         var name = string.Join(
                                                             string.Empty,
                                                             LinksV2.ToNameParts(LinksV2.GetFriendlyName(x.Actor))
                                                                 .Except(LinksV2.ToNameParts(LinksV2.GetFriendlyName(target.Actor)))
                                                         );

                                                         return $"{name} = {char.ToLower(name[0])}{name.Substring(1)};";
                                                     })
                                                 )
                                             }}
                                          }
                                          """
                                    )!
                                );
                        }

                        return backlink;
                    });

                    return node
                        .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                        .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                        .WithSemicolonToken(default)
                        .AddMembers(
                            hierarchy
                        );
                }
            );

        ApplyGetMethod(ref syntax, target, children, logger, context);
    }
}