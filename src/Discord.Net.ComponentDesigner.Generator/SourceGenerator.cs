using Discord.ComponentDesignerGenerator.Nodes;
using Discord.ComponentDesignerGenerator.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace Discord.ComponentDesignerGenerator;

public sealed record Target(
    InterceptableLocation InterceptLocation,
    InvocationExpressionSyntax InvocationSyntax,
    ExpressionSyntax ArgumentExpressionSyntax,
    IOperation Operation,
    Compilation Compilation,
    string? ParentKey,
    string CXDesigner,
    TextSpan CXDesignerSpan,
    DesignerInterpolationInfo[] Interpolations
)
{
    public SyntaxTree SyntaxTree => InvocationSyntax.SyntaxTree;
}

public sealed record DesignerInterpolationInfo(
    int Id,
    TextSpan Span,
    ITypeSymbol? Symbol,
    Optional<object?> Constant
);

[Generator]
public sealed class SourceGenerator : IIncrementalGenerator
{
    private readonly Dictionary<string, CXGraphManager> _cache = [];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context
            .SyntaxProvider
            .CreateSyntaxProvider(
                IsComponentDesignerCall,
                MapPossibleComponentDesignerCall
            )
            .Collect();

        context.RegisterSourceOutput(
            provider
                .Combine(provider.Select(GetKeysAndUpdateCachedEntries))
                .SelectMany(MapManagers)
                .Select((x, _) => x.Render())
                .Collect(),
            Generate
        );
    }

    private void Generate(SourceProductionContext context, ImmutableArray<RenderedInterceptor> interceptors)
    {
        if (interceptors.Length is 0) return;

        var sb = new StringBuilder();

        foreach (var interceptor in interceptors)
        {
            foreach (var diagnostic in interceptor.Diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }

            sb.AppendLine(
                $$"""
                  [global::System.Runtime.CompilerServices.InterceptsLocation(version: {{interceptor.Location.Version}}, data: "{{interceptor.Location.Data}}")]
                  public static global::Discord.ComponentBuilderV2 _{{Math.Abs(interceptor.GetHashCode())}}(
                      global::{{Constants.COMPONENT_DESIGNER_QUALIFIED_NAME}} designer
                  ) => new(
                      {{interceptor.Source.WithNewlinePadding(4)}}
                  )
                  """
            );
        }

        context.AddSource(
            "Interceptors.g.cs",
            $$"""
              using Discord;

              namespace System.Runtime.CompilerServices
              {
                  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
                  sealed file class InterceptsLocationAttribute(int version, string data) : Attribute;
              }

              namespace InlineComponent
              {
                  static file class Interceptors
                  {
                      {{sb.ToString().WithNewlinePadding(8)}}
                  }
              }
              """
        );
    }

    private IEnumerable<CXGraphManager> MapManagers(
        (ImmutableArray<Target?> targets, ImmutableArray<string?> keys) tuple,
        CancellationToken token
    )
    {
        var (targets, keys) = tuple;

        for (var i = 0; i < targets.Length; i++)
        {
            var target = targets[i];
            var key = keys[i];

            if (target is null || key is null) continue;

            // TODO: handle key updates

            if (_cache.TryGetValue(key, out var manager))
            {
                manager = _cache[key] = manager.OnUpdate(key, target);
            }
            else
            {
                manager = _cache[key] = CXGraphManager.Create(
                    this,
                    key,
                    target
                );
            }

            yield return manager;
        }
    }

    private ImmutableArray<string?> GetKeysAndUpdateCachedEntries(ImmutableArray<Target?> target,
        CancellationToken token)
    {
        var result = new string?[target.Length];

        var map = new Dictionary<string, int>();
        var globalCount = 0;

        for (var i = 0; i < target.Length; i++)
        {
            var targetItem = target[i];

            if (targetItem is null) continue;

            string key;
            if (targetItem.ParentKey is null)
            {
                key = $"<global>:{globalCount++}";
            }
            else
            {
                map.TryGetValue(targetItem.ParentKey, out var index);

                key = $"{targetItem.ParentKey}:{index}";
                map[targetItem.ParentKey] = index + 1;
            }

            result[i] = key;
        }

        foreach (var key in _cache.Keys.Except(result))
        {
            if (key is not null) _cache.Remove(key);
        }

        return [..result];
    }

    private static void OnTargetUpdated(Target? target, CancellationToken token)
    {
        if (target is null) return;

        //target.Compilation.SyntaxTrees
    }


    private static void ProcessTargetsUpdate(ImmutableArray<Target?> targets, CancellationToken token)
    {
        foreach (var target in targets)
        {
            if (target is null) continue;
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

        if (
            !TryGetCXDesigner(
                argumentSyntax,
                context.SemanticModel,
                out var cxDesigner,
                out var span,
                out var interpolationInfos
            )
        ) return null;


        return new Target(
            interceptLocation,
            invocationSyntax,
            argumentSyntax,
            operation,
            context.SemanticModel.Compilation,
            context.SemanticModel
                .GetEnclosingSymbol(invocationSyntax.SpanStart, token)
                ?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            cxDesigner,
            span,
            interpolationInfos
        );

        static bool TryGetCXDesigner(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            out string content,
            out TextSpan span,
            out DesignerInterpolationInfo[] interpolations
        )
        {
            switch (expression)
            {
                case LiteralExpressionSyntax {Token.Value: string literalContent} literal:
                    content = literalContent;
                    interpolations = [];
                    span = literal.Token.Span;
                    return true;

                case InterpolatedStringExpressionSyntax interpolated:
                    content = interpolated.Contents.ToString();
                    interpolations = interpolated.Contents
                        .OfType<InterpolationSyntax>()
                        .Select((x, i) => new DesignerInterpolationInfo(
                            i,
                            x.FullSpan,
                            semanticModel.GetTypeInfo(x.Expression).Type,
                            semanticModel.GetConstantValue(x.Expression)
                        ))
                        .ToArray();
                    span = interpolated.Contents.Span;
                    return true;
                default:
                    content = string.Empty;
                    span = default;
                    interpolations = [];
                    return false;
            }
        }

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
