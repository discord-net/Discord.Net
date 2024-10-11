using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Discord.Net.Hanz.Tasks.Actors;

public static class LinkMethods
{
    public static void Apply(
        SourceProductionContext context,
        LinksV2.GenerationTarget target,
        Logger logger
    )
    {
        ClassDeclarationSyntax? linkMethodsClass = null;

        foreach (var method in target.Actor.GetMembers().OfType<IMethodSymbol>())
        {
            var backlinkAttribute = method.GetAttributes()
                .FirstOrDefault(x => x.AttributeClass is {Name: "BackLinkAttribute", TypeArguments.Length: 1});


            if (backlinkAttribute is null)
                continue;

            var methodName = backlinkAttribute.ConstructorArguments[0].Value as string ?? method.Name;

            if (
                method.DeclaringSyntaxReferences
                    .FirstOrDefault()
                    ?.GetSyntax() is not MethodDeclarationSyntax methodSyntax
            ) continue;

            linkMethodsClass ??= (ClassDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
                $$"""
                  public static partial class {{LinksV2.GetFriendlyName(target.Actor)}}Extensions
                  {
                      
                  }
                  """
            )!;

            var carriedModifiers = SyntaxFactory.TokenList();

            if (methodSyntax.Modifiers.IndexOf(SyntaxKind.AsyncKeyword) != -1)
                carriedModifiers = carriedModifiers.Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword));

            var backlinkType =
                $"IBackLink<{backlinkAttribute.AttributeClass!.TypeArguments[0]}, {target.Actor}, {target.Id}, {target.Entity}, {target.Model}>";

            var parameters = new List<string>()
                {$"this {backlinkType} __link__"};

            IParameterSymbol? sourceParameter = null;
            IParameterSymbol? linkParameter = null;

            for (var i = 0; i < methodSyntax.ParameterList.Parameters.Count; i++)
            {
                var parameterSyntax = methodSyntax.ParameterList.Parameters[i];
                var parameter = method.Parameters[i];

                if (sourceParameter is null && parameter.Type.Equals(backlinkAttribute.AttributeClass!.TypeArguments[0],
                        SymbolEqualityComparer.Default))
                {
                    sourceParameter = parameter;
                    continue;
                }

                if (
                    linkParameter is null
                    &&
                    (
                        parameter.Type.Name == $"{LinksV2.GetFriendlyName(target.Actor)}Link"
                        ||
                        (
                            parameter.Type is INamedTypeSymbol {Name: "ILink", TypeArguments.Length: 4} link
                            &&
                            link.TypeArguments[0].Equals(target.Actor, SymbolEqualityComparer.Default)
                        )
                    )
                )
                {
                    linkParameter = parameter;
                    continue;
                }

                parameters.Add(parameterSyntax.ToString());
            }

            var methodBody = (SyntaxNode?) methodSyntax.ExpressionBody ?? methodSyntax.Body!;

            methodBody = methodBody.ReplaceNodes(
                methodBody.DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .Where(x =>
                        target.SemanticModel.GetOperation(x) is IParameterReferenceOperation
                    ),
                (node, _) =>
                {
                    var parameterRef = 
                        ((IParameterReferenceOperation) target.SemanticModel.GetOperation(node)!)
                        .Parameter;

                    if (parameterRef.Equals(sourceParameter, SymbolEqualityComparer.Default))
                    {
                        return SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("__link__"),
                            SyntaxFactory.IdentifierName("Source")
                        );
                    }

                    if (parameterRef.Equals(linkParameter, SymbolEqualityComparer.Default))
                    {
                        return SyntaxFactory.IdentifierName("__link__");
                    }

                    return node;
                }
            );

            linkMethodsClass = linkMethodsClass.AddMembers(
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      public static {{method.ReturnType}} {{methodName}}(
                          {{string.Join(", ", parameters)}}
                      )
                      {{methodBody}}
                      """
                )!.AddModifiers(carriedModifiers.ToArray())
            );
        }

        if (linkMethodsClass is not null)
        {
            context.AddSource(
                $"LinkMethods/{target.Actor.ToFullMetadataName()}",
                $$"""
                  {{target.Syntax.GetFormattedUsingDirectives()}}

                  namespace {{target.Actor.ContainingNamespace}};

                  {{linkMethodsClass.NormalizeWhitespace()}}
                  """
            );
        }
    }
}