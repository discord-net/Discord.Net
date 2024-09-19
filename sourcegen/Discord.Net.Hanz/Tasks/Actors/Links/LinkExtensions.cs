using System.Collections.Immutable;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors;

using ExtensionMember = (IPropertySymbol Property, LinksV2.GenerationTarget? Target, string Type);
using ExtensionMembers = (IPropertySymbol Property, LinksV2.GenerationTarget? Target, string Type)[];

public sealed class LinkExtensions
{
    private static ExtensionMembers GetExtensionMembers(
        INamedTypeSymbol extension,
        ImmutableArray<LinksV2.GenerationTarget?> targets)
    {
        return extension
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Select(x =>
                (
                    Property: x,
                    Target: targets
                        .FirstOrDefault(y =>
                            y?.Actor.Equals(x.Type, SymbolEqualityComparer.Default) ?? false)
                )
            )
            .Select(x =>
                (
                    x.Property,
                    x.Target,
                    Type: x.Target is not null
                        ? $"{LinksV2.GetFriendlyName(x.Target!.Actor)}Link"
                        : x.Property.Type.ToDisplayString()
                )
            )
            .ToArray();
    }

    public static void Process<T>(
        ref T syntax,
        LinksV2.GenerationTarget target,
        ImmutableArray<LinksV2.GenerationTarget?> targets,
        Logger logger
    )
        where T : TypeDeclarationSyntax
    {
        if (syntax.Identifier.ValueText is "BackLink")
        {
            logger.Log($"{target.Actor}: Skipping backlink for link extension");
            return;
        }

        var extensions = 
            (target.Assembly is LinksV2.AssemblyTarget.Core ? target.Actor : target.GetCoreActor())
            .GetTypeMembers()
            .Where(x => x
                .GetAttributes()
                .Any(x => x.AttributeClass?.Name == "LinkExtensionAttribute")
            );

        foreach (var extension in extensions)
        {
            var name = extension.Name.Replace("Extension", string.Empty);

            logger.Log($"{target.Actor}: Processing extension {name} ({extension})");

            var members = GetExtensionMembers(extension, targets);

            if (members.Length == 0)
            {
                logger.Log($"{target.Actor}: {extension} has no valid members, ignoring");
                continue;
            }

            var extensionSyntax = (T) SyntaxFactory.ParseMemberDeclaration(
                $$"""
                    public {{(syntax is ClassDeclarationSyntax ? "class" : "interface")}} {{name}}
                    {
                        {{
                            string.Join(
                                Environment.NewLine,
                                members.Select(x =>
                                {
                                    var shouldBeNew =
                                        members.Count(y => y.Property.Name == x.Property.Name) > 1
                                        && x.Property.ExplicitInterfaceImplementations.Length == 0;

                                    if (x.Property.ExplicitInterfaceImplementations.Length == 0)
                                        return $"{(shouldBeNew ? "new " : string.Empty)}{x.Type} {x.Property.Name} {{ get; }}";

                                    var baseProp = x.Property.ExplicitInterfaceImplementations.First();

                                    var baseTarget = targets
                                        .FirstOrDefault(y =>
                                            y is not null &&
                                            y.Actor.Equals(
                                                baseProp.Type,
                                                SymbolEqualityComparer.Default
                                            )
                                        );

                                    if (baseTarget is null)
                                        return $"// unknown member {x.Property}";

                                    return $"{LinksV2.GetFriendlyName(baseTarget.Actor)}Link {baseProp.ContainingType}.{baseProp.Name} => {x.Property.Name};";
                                })
                            )
                        }}
                    }   
                  """
            )!;

            var baseExtensions = extension
                .Interfaces
                .Where(x => x
                    .GetAttributes()
                    .Any(x => x.AttributeClass?.Name == "LinkExtensionAttribute")
                )
                .ToArray();

            if (baseExtensions.Length > 0)
            {
                extensionSyntax = (T) extensionSyntax
                    .AddBaseListTypes(
                        baseExtensions
                            .Select(x =>
                                SyntaxFactory.SimpleBaseType(
                                    SyntaxFactory.ParseTypeName(
                                        x.ToDisplayString().Replace("Extension", string.Empty)
                                    )
                                )
                            )
                            .ToArray<BaseTypeSyntax>()
                    );
            }

            syntax = (T) syntax
                .AddMembers(
                    extensionSyntax
                );

            syntax = syntax.ReplaceNodes(
                syntax
                    .DescendantNodes()
                    .OfType<T>(),
                (old, node) =>
                {
                    if (old.Identifier.ValueText == name) return node;

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

                    var extensionSyntax = (T) SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          public {{(old is ClassDeclarationSyntax ? "class" : "interface")}} {{name}} : {{path}}, {{target.Actor}}.{{name}} 
                          {
                              {{
                                  string.Join(
                                      Environment.NewLine,
                                      members.Select(x =>
                                      {
                                          if (x.Property.ExplicitInterfaceImplementations.Length == 0)
                                          {
                                              return target.Assembly is LinksV2.AssemblyTarget.Core
                                                  ? $$"""
                                                      new {{x.Property.Type}} {{x.Property.Name}} { get; }
                                                      {{x.Type}} {{target.Actor}}.{{name}}.{{x.Property.Name}} => {{x.Property.Name}};
                                                      """
                                                  : $$"""
                                                      public {{x.Property.Type}} {{x.Property.Name}} { get; }
                                                      {{x.Type}} {{target.Actor}}.{{name}}.{{x.Property.Name}} => {{x.Property.Name}};
                                                      """;
                                          }

                                          var baseProp = x.Property.ExplicitInterfaceImplementations.First();

                                          return
                                              $$"""
                                                {{x.Property.Type}} {{baseProp.ContainingType.ContainingType}}.{{path}}.{{baseProp.ContainingType.Name.Replace("Extension", string.Empty)}}.{{MemberUtils.GetMemberName(baseProp)}} => {{MemberUtils.GetMemberName(x.Property)}};
                                                """;
                                      })
                                  )
                              }}
                          }
                          """
                    )!;

                    if (baseExtensions.Length > 0)
                    {
                        extensionSyntax = (T)extensionSyntax
                            .AddBaseListTypes(
                                baseExtensions
                                    .Select(x =>
                                        SyntaxFactory.SimpleBaseType(
                                            SyntaxFactory.ParseTypeName(
                                                $"{x.ContainingType}.{path}.{x.Name.Replace("Extension", string.Empty)}"
                                            )
                                        )
                                    )
                                    .ToArray<BaseTypeSyntax>()
                            );
                    }

                    LinksV2.AddBackLink(ref extensionSyntax!, target, logger, false, false);

                    return node
                        .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                        .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                        .WithSemicolonToken(default)
                        .AddMembers(
                            extensionSyntax
                        );
                }
            );
        }
    }
}