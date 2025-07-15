using System.Collections.Immutable;
using Discord.Net.Hanz.Nodes;
using Discord.Net.Hanz.Utils;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Discord.Net.Hanz.Tasks.Actors.Links.Methods;

public sealed class BackLinkMethodNode : Node
{
    private record Context(
        string From,
        MethodTemplateSpec Spec,
        ImmutableEquatableArray<(string Target, string MethodName)> Targets
    );

    private record BackLinkTarget(
        ActorInfo ActorInfo,
        MethodTemplateSpec Spec,
        ActorInfo BackLinkActorInfo,
        string MethodName
    );

    public BackLinkMethodNode(IncrementalGeneratorInitializationContext context, ILogger logger) : base(context, logger)
    {
        context.RegisterSourceOutput(
            context
                .SyntaxProvider
                .ForAttributeWithMetadataName(
                    "Discord.BackLinkAttribute`1",
                    (node, _) => node is MethodDeclarationSyntax,
                    Map
                )
                .WhereNotNull()
                .GroupBy(x => x.From)
                .TransformKeysVia(GetTask<ActorsTask>().ActorInfos)
                .TransformValues((info, contexts) => contexts
                    .SelectMany(x => x
                        .Targets
                        .Where(backLink => GetTask<ActorsTask>().ActorInfos.ContainsKey(backLink.Target))
                        .Select(backLink =>
                            new BackLinkTarget(
                                info,
                                x.Spec,
                                GetTask<ActorsTask>().ActorInfos.GetValue(backLink.Target),
                                backLink.MethodName
                            )
                        )
                    )
                )
                .MapValues((info, target) => (CreateMethodSpec(target), target.Spec.Usings))
                .Select(CreateSourceSpec)
        );
    }

    private static MethodSpec CreateMethodSpec(BackLinkTarget target)
    {
        return target.Spec.Template with
        {
            Parameters = new([
                ($"this {target.ActorInfo.FormattedBackLinkOfType(target.BackLinkActorInfo.Actor)}", "__link__"),
                ..target.Spec.Template.Parameters
            ]),
            Name = target.MethodName,
            Expression = target.Spec.Expression,
            Body = target.Spec.Body
        };
    }

    private static SourceSpec CreateSourceSpec(
        ActorInfo actorInfo,
        ImmutableArray<(MethodSpec Spec, ImmutableEquatableArray<string> Usings)> methods)
    {
        return new(
            $"LinkMethods/BackLink_{actorInfo.Actor.MetadataName}",
            actorInfo.Actor.Namespace!,
            methods
                .SelectMany(x => x.Usings)
                .Prepend("Discord")
                .Prepend("Discord.Models")
                .Distinct()
                .ToImmutableEquatableArray(),
            new([
                new TypeSpec(
                    $"{GetFriendlyName(actorInfo.Actor)}BackLinkExtensions",
                    Kind: TypeKind.Class,
                    Modifiers: new([
                        "static"
                    ]),
                    Methods: methods.Select(x => x.Spec).ToImmutableEquatableArray()
                )
            ])
        );
    }


    private static Context? Map(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        if (context.TargetNode is not MethodDeclarationSyntax syntax)
            return null;

        if (syntax.Parent is not TypeDeclarationSyntax typeSyntax)
            return null;

        if (context.SemanticModel.GetDeclaredSymbol(typeSyntax) is not { } symbol)
            return null;

        if (context.TargetSymbol is not IMethodSymbol methodSymbol)
            return null;

        if (methodSymbol.Parameters.Length == 0)
            return null;

        if (syntax.Modifiers.IndexOf(SyntaxKind.StaticKeyword) == -1)
            return null;

        var targets = context
            .Attributes
            .Where(x =>
                x.AttributeClass is not null &&
                (
                    x.AttributeClass.TypeArguments[0].Equals(methodSymbol.Parameters[0].Type,
                        SymbolEqualityComparer.Default)
                    ||
                    context.SemanticModel.Compilation.HasImplicitConversion(
                        x.AttributeClass.TypeArguments[0],
                        methodSymbol.Parameters[0].Type
                    )
                )
            )
            .Select(x =>
                (
                    x.AttributeClass!.TypeArguments[0].ToDisplayString(),
                    x.ConstructorArguments.FirstOrDefault().Value as string ?? syntax.Identifier.ValueText
                )
            )
            .ToImmutableEquatableArray();

        if (targets.Count == 0)
            return null;

        if (MapSpec(syntax, methodSymbol, context.SemanticModel) is not { } spec)
            return null;

        return new(
            symbol.ToDisplayString(),
            spec,
            targets
        );
    }

    private static MethodTemplateSpec? MapSpec(
        MethodDeclarationSyntax syntax,
        IMethodSymbol symbol,
        SemanticModel model)
    {
        var methodImpl =
            (SyntaxNode?) syntax.Body
            ?? syntax.ExpressionBody;

        if (methodImpl is null) return null;

        var linkParameter = symbol.Parameters.FirstOrDefault(x => x.Type.Name == "Link");

        var template = methodImpl
            .ReplaceNodes(
                methodImpl.DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .Where(x => model.GetOperation(x) is IParameterReferenceOperation),
                (node, _) =>
                {
                    var parameterRef =
                        ((IParameterReferenceOperation) model.GetOperation(node)!)
                        .Parameter;

                    if (parameterRef.Equals(symbol.Parameters[0], SymbolEqualityComparer.Default))
                        return SyntaxFactory
                            .MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("__link__"),
                                SyntaxFactory.IdentifierName("Source")
                            )
                            .WithTriviaFrom(node);

                    if (parameterRef.Equals(linkParameter, SymbolEqualityComparer.Default))
                        return SyntaxFactory.IdentifierName("__link__").WithTriviaFrom(node);

                    return node;
                }
            )
            .ToString();

        if (syntax.Body is not null)
        {
            template = template
                .Remove(0, 1)
                .Remove(template.Length - 2, 1);

            var split = template.Split([Environment.NewLine], StringSplitOptions.None)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            if (split.Length > 0)
            {
                var max = split.Min(x => x.TakeWhile(char.IsWhiteSpace).Count());
                template = string.Join(Environment.NewLine, split.Select(x => x.Length >= max ? x.Remove(0, max) : x));
            }
        }

        var methodParameters = symbol.Parameters.RemoveAt(0);

        if (linkParameter is not null)
            methodParameters = methodParameters.RemoveAt(symbol.Parameters.IndexOf(linkParameter) - 1);

        return new MethodTemplateSpec(
            syntax.Body is not null
                ? template
                : null,
            syntax.ExpressionBody is not null ? template : null,
            syntax
                .GetUsingDirectivesSyntax()
                .Select(x => x.Name?.ToString())
                .Where(x => x is not null)!
                .ToImmutableEquatableArray<string>(),
            MethodSpec.From(symbol) with
            {
                Accessibility = Accessibility.Public,
                Modifiers = new([
                    "static",
                    ..syntax.Modifiers.IndexOf(SyntaxKind.AsyncKeyword) != -1 ? (string[]) ["async"] : []
                ]),
                Parameters = methodParameters.Select(ParameterSpec.From).ToImmutableEquatableArray()
            }
        );
    }
}