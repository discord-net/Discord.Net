using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Discord.ComponentDesignerGenerator;

public sealed record Target(
    InterceptableLocation InterceptLocation,
    InvocationExpressionSyntax InvocationSyntax,
    ExpressionSyntax ArgumentExpressionSyntax,
    IOperation Operation,
    Compilation Compilation
);

public sealed class SourceManager
{
    public SourceManager(IncrementalGeneratorInitializationContext context)
    {
        context
            .SyntaxProvider
            .CreateSyntaxProvider(
                IsComponentDesignerCall,
                MapPossibleComponentDesignerCall
            )
            .Collect();
    }

    private static void ProcessTargetsUpdate(ImmutableArray<Target?> targets, CancellationToken token)
    {
        foreach (var target in targets)
        {
            if(target is null) continue;

            
        }
    }


    private static Target? MapPossibleComponentDesignerCall(GeneratorSyntaxContext context, CancellationToken token)
    {
        if (
            !TryGetValidDesignerCall(
                out var operation,
                out var invocationSyntax,
                out var interceptLocation,
                out var argumentSyntax
            )
        ) return null;

        return new Target(
            interceptLocation,
            invocationSyntax,
            argumentSyntax,
            operation,
            context.SemanticModel.Compilation
        );


        bool TryGetValidDesignerCall(
            out IOperation operation,
            out InvocationExpressionSyntax invocationSyntax,
            out InterceptableLocation interceptLocation,
            out ExpressionSyntax argumentExpressionSyntax
        )
        {
            operation = context.SemanticModel.GetOperation(context.Node, token)!;
            interceptLocation = null!;
            argumentExpressionSyntax = null!;
            invocationSyntax = null!;

            checkOperation:
            switch (operation)
            {
                case IInvalidOperation invalid:
                    operation = invalid.ChildOperations.OfType<IInvocationOperation>().FirstOrDefault()!;
                    goto checkOperation;
                case IInvocationOperation invocation:
                    if (
                        invocation
                            .TargetMethod
                            .ContainingType
                            .ToDisplayString()
                        is "Discord.ComponentDesigner"
                    ) break;
                    goto default;

                default: return false;
            }

            if (context.Node is not InvocationExpressionSyntax syntax) return false;

            invocationSyntax = syntax;

            if (context.SemanticModel.GetInterceptableLocation(invocationSyntax) is not { } location)
                return false;

            interceptLocation = location;

            if (invocationSyntax.ArgumentList.Arguments.Count is not 1) return false;

            argumentExpressionSyntax = invocationSyntax.ArgumentList.Arguments[0].Expression;

            return true;
        }
    }

    private static bool IsComponentDesignerCall(SyntaxNode node, CancellationToken token)
        => node is InvocationExpressionSyntax
        {
            Expression: MemberAccessExpressionSyntax
            {
                Name: {Identifier.Value: "Create" or "cx"}
            } or IdentifierNameSyntax
            {
                Identifier.ValueText: "cx"
            }
        };
}
