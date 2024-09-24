using System.Collections.Immutable;
using System.Text;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors;

using ExtensionProperty =
    (bool IsBackLinkMirror, bool IsLinkMirror, IPropertySymbol Property, LinksV2.GenerationTarget? Target);
using ExtensionProperties =
    (bool IsBackLinkMirror, bool IsLinkMirror, IPropertySymbol Property, LinksV2.GenerationTarget? Target)[];
using FormattedMember = (string Type, string Name, string Properties);

public sealed class LinkExtensions
{
    private static ExtensionProperties GetExtensionProperties(
        INamedTypeSymbol extension,
        ImmutableArray<LinksV2.GenerationTarget?> targets)
    {
        return extension
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Select(x => ParseProperty(x, targets))
            .ToArray();
    }

    private static ExtensionProperty ParseProperty(
        IPropertySymbol property,
        ImmutableArray<LinksV2.GenerationTarget?> targets)
    {
        var mirrorAttribute = property.GetAttributes()
            .FirstOrDefault(v => v.AttributeClass?.Name == "LinkMirrorAttribute");
        return (
            IsBackLinkMirror: mirrorAttribute?.NamedArguments
                .FirstOrDefault(x => x.Key == "OnlyBackLinks")
                .Value.Value as bool? == true,
            IsLinkMirror: mirrorAttribute is not null,
            Property: property,
            Target: targets
                .FirstOrDefault(y =>
                    y is not null &&
                    y.Assembly switch
                    {
                        LinksV2.AssemblyTarget.Core => y.Actor.Equals(property.Type, SymbolEqualityComparer.Default),
                        _ => y.GetCoreActor().ToDisplayString().Equals(property.Type.ToDisplayString())
                    }
                )
        );
    }

    private static FormattedMember FormatMember(
        string extension,
        ExtensionProperty member,
        ExtensionProperties properties,
        LinksV2.GenerationTarget target,
        ImmutableArray<LinksV2.GenerationTarget?> targets,
        Logger logger,
        string? path = null,
        string? pathWithExtensions = null,
        bool shouldBeNew = false,
        string? modifier = null)
    {
        // validate the member
        if (member is {IsLinkMirror: true, Target: null})
        {
            logger.Warn(
                $"Unknown target for mirror prop {member.Property.Name} ({member.Property.Type}) in {member.Property.ContainingType}");
            return default;
        }

        var result = new StringBuilder();
        var type = new StringBuilder();
        var name = MemberUtils.GetMemberName(member.Property);
        shouldBeNew |=
            target.Assembly is LinksV2.AssemblyTarget.Core &&
            member.Property.ExplicitInterfaceImplementations.Length == 0 &&
            properties.Any(x =>
                MemberUtils.GetMemberName(x.Property) == member.Property.Name &&
                x.Property.ExplicitInterfaceImplementations.Length > 0
            );

        if (target.Assembly is LinksV2.AssemblyTarget.Core)
        {
            if (member.Property.ExplicitInterfaceImplementations.Length > 0)
            {
                var ifaceImpl = ParseProperty(
                    member.Property.ExplicitInterfaceImplementations[0],
                    targets
                );

                var baseExtension = ifaceImpl.Property.ContainingType.ToDisplayString()
                    .Replace("Extension", string.Empty);

                if (member is {IsLinkMirror: true, Target: not null} &&
                    ifaceImpl is {IsLinkMirror: true, Target: not null})
                {
                    // can safely add paths to both
                    result
                        .Append($"{ifaceImpl.Target.Actor}.{path} ")
                        .Append($"{baseExtension}.{ifaceImpl.Property.Name} ")
                        .AppendLine($"=> {name};");
                }
                else if (!member.IsLinkMirror && !ifaceImpl.IsLinkMirror)
                {
                    if (path is not null) return default;

                    result
                        .Append($"{ifaceImpl.Property.Type} ")
                        .Append($"{baseExtension}.{name} ")
                        .AppendLine($"=> {name};");
                }
                else
                {
                    logger.Warn($"Unknown resolution for {member.Property} <-> {ifaceImpl.Property}");
                    return default;
                }

                return (
                    Type: member.Property.Type.ToDisplayString(),
                    Name: name,
                    Properties: result.ToString()
                );
            }

            type.Append(
                member.Target is not null
                    ? member.Target.Actor.ToDisplayString()
                    : member.Property.Type.ToDisplayString()
            );

            switch (member.IsLinkMirror)
            {
                case true when member.IsBackLinkMirror && path == "BackLink<TSource>":
                    type.Append($".{path}");
                    result.AppendLine($"{member.Property.Type} {target.Actor}.{extension}.{name} => {name};");
                    break;
                case true when member.IsBackLinkMirror && path is null:
                    break;
                case true when member.IsBackLinkMirror && path != "BackLink<TSource>":
                    // no need to override
                    return default;
                case true when path is not null:
                    shouldBeNew = true;

                    // override
                    result.AppendLine(
                        $"{LinksV2.GetFriendlyName(member.Target!.Actor)}Link {member.Target.Actor}.{extension}.{name} => {name};"
                    );

                    type.Append($".{path}");
                    break;
                case true when !member.IsBackLinkMirror:
                    // add basic link type
                    type.Clear().Append($"{LinksV2.GetFriendlyName(member.Target!.Actor)}Link");
                    break;
                case false when path is not null:
                    return default;
            }
        }
        else
        {
            if (member.Property.ExplicitInterfaceImplementations.Length > 0)
                return default;

            if (member.Target is not null)
            {
                var coreType = new StringBuilder(member.Target.GetCoreActor().ToDisplayString());
                var extType = new StringBuilder(target.GetCoreActor().ToDisplayString());

                if (member.IsLinkMirror)
                {
                    if (!member.IsBackLinkMirror || path == "BackLink<TSource>")
                        coreType.Append($".{path}");

                    extType.Append($".{pathWithExtensions}");
                }

                result.AppendLine(
                    $"{coreType} {extType}.{extension}.{name} => {name};"
                );

                type.Append($"{member.Target.Actor}");
            }
            else if (member.Property.Type.ContainingType is not null)
            {
                var str = member.Property.Type.ToDisplayString();
                var containingTarget =
                    targets.FirstOrDefault(x =>
                        x is not null &&
                        str.StartsWith(x.GetCoreActor().ToDisplayString())
                    );

                if (containingTarget is not null)
                {
                    result.AppendLine(
                        $"{member.Property.Type} {containingTarget.GetCoreActor()}.{(path is not null && member.IsLinkMirror ? $"{path}." : string.Empty)}{extension}.{name} => {name};"
                    );

                    type.Append(
                        str.Replace(
                            containingTarget.GetCoreActor().ToDisplayString(),
                            containingTarget.Actor.ToDisplayString()
                        )
                    );
                }
                else
                {
                    type.Append($"{member.Property.Type}");
                }
            }
            else
            {
                type.Append($"{member.Property.Type}");
            }

            if (
                path is not null &&
                (
                    member is {IsBackLinkMirror: false, IsLinkMirror: true}
                    ||
                    (
                        member.IsBackLinkMirror &&
                        path == "BackLink<TSource>"
                    )
                )
            )
            {
                type.Append($".{path}");
            }
        }

        var property = new StringBuilder();

        if (target.Assembly is not LinksV2.AssemblyTarget.Core)
            property.Append("public ");

        if (modifier is not null)
            property.Append(modifier).Append(' ');
        else if (shouldBeNew)
            property.Append("new ");

        property.Append($"{type} {name} {{ get; }}");

        return (
            Type: type.ToString(),
            Name: name,
            Properties: result.AppendLine(property.ToString()).ToString()
        );
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

            var members = GetExtensionProperties(extension, targets);

            if (members.Length == 0)
            {
                logger.Log($"{target.Actor}: {extension} has no valid members, ignoring");
                continue;
            }

            if (target.Assembly is LinksV2.AssemblyTarget.Core)
            {
                var extensionSyntax = (T) SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                        public interface {{name}}
                        {
                            {{
                                string.Join(
                                    Environment.NewLine,
                                    members.Select(x => FormatMember(
                                        name,
                                        x,
                                        members,
                                        target,
                                        targets,
                                        logger
                                    ).Properties)
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
            }

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
                        .TakeWhile(x => x.Identifier.ValueText != target.Actor.Name)
                        .ToList();

                    var anscestorsWithoutExtensions = anscestors
                        .Where(x => x.AttributeLists.All(x =>
                            x.Attributes.All(x => x.Name.ToString() != "LinkExtension")))
                        .ToList();

                    var path = string.Join(
                        ".",
                        anscestors
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

                    var pathWithoutExtensions = string.Join(
                        ".",
                        anscestorsWithoutExtensions
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

                    var backlinkMirrorProps = members.Where(x => x.IsBackLinkMirror).ToArray();

                    var formattedMembers = members
                        .Select(x =>
                            (
                                Formatted: FormatMember(
                                    name,
                                    x,
                                    members,
                                    target,
                                    targets,
                                    logger,
                                    pathWithoutExtensions,
                                    path,
                                    modifier:
                                    target.Assembly is not LinksV2.AssemblyTarget.Core &&
                                    x.IsBackLinkMirror
                                        ? "virtual"
                                        : null
                                ),
                                Raw: x
                            )
                        )
                        .Where(x => x.Formatted.Properties is not null)
                        .ToArray();

                    var extensionSyntax = (T) SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          [LinkExtension]
                          public {{(old is ClassDeclarationSyntax ? "class" : "interface")}} {{name}} : {{path}}, {{(
                              target.Assembly is LinksV2.AssemblyTarget.Core ? $"{target.Actor}.{name}" : $"{target.GetCoreActor()}.{path}.{name}"
                          )}}
                          {
                              {{
                                  string.Join(
                                      Environment.NewLine,
                                      formattedMembers
                                          .Select(x => x.Formatted.Properties)
                                  )
                              }}
                          }
                          """
                    )!;

                    if (old is ClassDeclarationSyntax cls)
                    {
                        foreach (
                            var ctor
                            in cls.Members.OfType<ConstructorDeclarationSyntax>()
                                .Select(x => x.ParameterList)
                                .Prepend(cls.ParameterList)
                        )
                        {
                            if (ctor is null) continue;

                            extensionSyntax = (T) extensionSyntax
                                .AddMembers(
                                    SyntaxFactory.ParseMemberDeclaration(
                                        $$"""
                                           public {{name}}{{ctor
                                               .AddParameters(formattedMembers
                                                   .Select(x => SyntaxFactory.Parameter(
                                                       [],
                                                       [],
                                                       SyntaxFactory.ParseTypeName(
                                                           $"{x.Formatted.Type}"
                                                       ),
                                                       SyntaxFactory.Identifier($"{char.ToLower(x.Formatted.Name[0])}{x.Formatted.Name.Substring(1)}"),
                                                       null
                                                   ))
                                                   .ToArray()
                                               )
                                               .NormalizeWhitespace()
                                           }} : base({{string.Join(", ", ctor.Parameters.Select(x => x.Identifier))}})
                                          {
                                           {{
                                               string.Join(
                                                   Environment.NewLine,
                                                   formattedMembers.Select(x =>
                                                       $"{x.Formatted.Name} = {char.ToLower(x.Formatted.Name[0])}{x.Formatted.Name.Substring(1)};"
                                                   )
                                               )
                                           }}
                                          }
                                          """
                                    )!
                                );
                        }
                    }

                    LinksV2.AddBackLink(
                        ref extensionSyntax,
                        target,
                        logger,
                        false,
                        false,
                        transformer: backlink =>
                        {
                            if (backlinkMirrorProps.Length == 0) return backlink;

                            backlink = (T) backlink
                                .AddMembers(
                                    backlinkMirrorProps
                                        .Select(x =>
                                            FormatMember(
                                                name,
                                                x,
                                                members,
                                                target,
                                                targets,
                                                logger,
                                                $"BackLink<TSource>",
                                                $"BackLink<TSource>",
                                                true,
                                                modifier: target.Assembly is not LinksV2.AssemblyTarget.Core &&
                                                          x.IsBackLinkMirror
                                                    ? "override"
                                                    : null
                                            )
                                        )
                                        .SelectMany(x => x.Properties
                                            .Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries)
                                            .Select(x => SyntaxFactory.ParseMemberDeclaration(x)!)
                                        )
                                        .ToArray()
                                );

                            if (node is ClassDeclarationSyntax)
                            {
                                backlink = backlink.ReplaceNodes(
                                    backlink.Members.OfType<ConstructorDeclarationSyntax>(),
                                    (_, node) =>
                                    {
                                        node = node
                                            .WithParameterList(
                                                node.ParameterList.ReplaceNodes(
                                                    node.ParameterList.Parameters.Where(x =>
                                                        formattedMembers.Any(y =>
                                                            y.Raw.IsBackLinkMirror &&
                                                            $"{char.ToLower(y.Formatted.Name[0])}{y.Formatted.Name.Substring(1)}" ==
                                                            x.Identifier.ValueText
                                                        )
                                                    ),
                                                    (_, node) =>
                                                    {
                                                        var parameter = formattedMembers.FirstOrDefault(y =>
                                                            y.Raw.IsBackLinkMirror &&
                                                            $"{char.ToLower(y.Formatted.Name[0])}{y.Formatted.Name.Substring(1)}" ==
                                                            node.Identifier.ValueText
                                                        );

                                                        if (parameter.Formatted.Name is null) return node;

                                                        return node.WithType(
                                                            SyntaxFactory.ParseTypeName(
                                                                $"{node.Type}.BackLink<TSource>")
                                                        );
                                                    }
                                                )
                                            );

                                        var body = node.Body ?? SyntaxFactory.Block();

                                        return node.WithBody(body
                                            .AddStatements(
                                                formattedMembers
                                                    .Where(x => x.Raw.IsBackLinkMirror)
                                                    .Select(x =>
                                                        SyntaxFactory.ExpressionStatement(
                                                            SyntaxFactory.ParseExpression(
                                                                $"{x.Formatted.Name} = {char.ToLower(x.Formatted.Name[0])}{x.Formatted.Name.Substring(1)}"
                                                            )
                                                        )
                                                    )
                                                    .ToArray<StatementSyntax>()
                                            )
                                        );
                                    }
                                );
                            }
                            else
                            {
                                backlink = (T) backlink
                                    .AddMembers(
                                        formattedMembers
                                            .Where(x => x.Raw.IsBackLinkMirror)
                                            .Select(x =>
                                                SyntaxFactory.ParseMemberDeclaration(
                                                    $"""
                                                     {x.Formatted.Type} {target.Actor}.{path}.{name}.{x.Formatted.Name} => {x.Formatted.Name};    
                                                     """
                                                )!
                                            )
                                            .ToArray()
                                    );
                            }

                            return (T) backlink
                                .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                                .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                                .WithSemicolonToken(default);
                        }
                    );

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