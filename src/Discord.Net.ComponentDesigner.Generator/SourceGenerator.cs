using Discord.ComponentDesignerGenerator.Nodes;
using Discord.ComponentDesignerGenerator.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace Discord.ComponentDesignerGenerator;

[Generator]
public sealed class SourceGenerator : IIncrementalGenerator
{
    private readonly record struct Target(
        InterceptableLocation InterceptableLocation,
        Location Location,
        string[] Content,
        InterpolationInfo[] Interpolations,
        bool IsMultiLine,
        KnownTypes KnownTypes,
        Func<string, ImmutableArray<ISymbol>> LookupNode
    )
    {
        public bool Equals(Target? other)
            => other is { } target &&
               InterceptableLocation.Equals(target.InterceptableLocation) &&
               Location.Equals(target.Location) &&
               Content.SequenceEqual(target.Content) &&
               Interpolations.SequenceEqual(target.Interpolations) &&
               IsMultiLine == target.IsMultiLine;

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = InterceptableLocation.GetHashCode();
                hashCode = (hashCode * 397) ^ Location.GetHashCode();
                hashCode = (hashCode * 397) ^ Content.Aggregate(0, (a, b) => (a * 397) ^ b.GetHashCode());
                hashCode = (hashCode * 397) ^ Interpolations.Aggregate(0, (a, b) => (a * 397) ^ b.GetHashCode());
                hashCode = (hashCode * 397) ^ IsMultiLine.GetHashCode();
                return hashCode;
            }
        }
    }

    private readonly record struct Interceptor(
        string? Source,
        Diagnostic[] Diagnostics
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context
            .SyntaxProvider
            .CreateSyntaxProvider((x, _) =>
                    x is InvocationExpressionSyntax
                    {
                        Expression: MemberAccessExpressionSyntax
                        {
                            Name: {Identifier.Value: "Create" or "cx"}
                        } or IdentifierNameSyntax
                        {
                            Identifier.ValueText: "cx"
                        }
                    },
                Transform
            )
            .Select(BuildInterceptor)
            .Collect();

        context.RegisterSourceOutput(provider, Generate);
    }

    private static Interceptor? BuildInterceptor(Target? rawTarget, CancellationToken token)
    {
        if (rawTarget is not { } target) return null;

        var diagnostics = new List<Diagnostic>();

        var interpolationLengths = target.Interpolations.Select(x => x.Length).ToArray();
        var doc = ComponentParser.Parse(target.Content, interpolationLengths);

        var componentContext = new ComponentNodeContext(
            doc,
            target.Location,
            target.IsMultiLine,
            target.Interpolations,
            target.KnownTypes,
            target.LookupNode
        );

        foreach (var parsingDiagnostic in doc.Diagnostics)
        {
            diagnostics.Add(
                Diagnostic.Create(
                    Diagnostics.ComponentParseError,
                    componentContext.GetLocation(parsingDiagnostic.Span),
                    parsingDiagnostic.Message
                )
            );
        }

        var nodes = doc
            .Elements
            .Select(x => ComponentNode.Create(x, componentContext))
            .Where(x => x is not null)
            .ToArray();

        foreach (var node in nodes)
        {
            node!.ReportValidationErrors();
        }

        diagnostics.AddRange(componentContext.Diagnostics);

        if (componentContext.HasErrors) return new(null, [..diagnostics]);

        return new Interceptor(
            $$"""
              [global::System.Runtime.CompilerServices.InterceptsLocation(version: {{target.InterceptableLocation.Version}}, data: "{{target.InterceptableLocation.Data}}")]
              public static global::Discord.ComponentBuilderV2 _{{Math.Abs(target.GetHashCode())}}(
                  global::{{Constants.COMPONENT_DESIGNER_QUALIFIED_NAME}} designer
              )
              {
                  return new([
                      {{
                          string.Join(
                              "\n".Postfix(8),
                              nodes.Select(x => x!.Render().WithNewlinePadding(8))
                          )
                      }}
                  ]);
              }
              """,
            [..diagnostics]
        );
    }

    private void Generate(SourceProductionContext context, ImmutableArray<Interceptor?> arg2)
    {
        var sb = new StringBuilder();

        foreach (var interceptor in arg2)
        {
            if (!interceptor.HasValue) continue;

            foreach (var diagnostic in interceptor.Value.Diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }

            if (interceptor.Value.Source is not null) sb.AppendLine(interceptor.Value.Source);
        }

        if (sb.Length is 0) return;

        context.AddSource(
            "Interceptors.g.cs",
            $$"""
              using Discord;

              namespace System.Runtime.CompilerServices
              {
                  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
                  sealed file class InterceptsLocationAttribute(int version, string data) : Attribute
                  {
                  }
              }

              namespace InlineComponent
              {
                  static file class Interceptors
                  {
                      {{sb.ToString().Replace("\n", "\n        ")}}
                  }
              }
              """
        );
    }

    private Target? Transform(GeneratorSyntaxContext context, CancellationToken token)
    {
        var operation = context.SemanticModel.GetOperation(context.Node, token);

        checkOperation:
        switch (operation)
        {
            case IInvalidOperation invalid:
                operation = invalid.ChildOperations.OfType<IInvocationOperation>().FirstOrDefault();
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

            default: return null;
        }

        if (context.Node is not InvocationExpressionSyntax invocationSyntax) return null;

        if (context.SemanticModel.GetInterceptableLocation(invocationSyntax) is not { } location)
            return null;

        if (invocationSyntax.ArgumentList.Arguments.Count is not 1) return null;

        var argument = invocationSyntax.ArgumentList.Arguments[0].Expression;

        var content = new List<string>();
        var interpolations = new List<InterpolationInfo>();
        var isMultiLine = false;

        switch (argument)
        {
            case InterpolatedStringExpressionSyntax interpolated:
                foreach (var interpolation in interpolated.Contents)
                {
                    switch (interpolation)
                    {
                        case InterpolatedStringTextSyntax interpolatedStringTextSyntax:
                            content.Add(interpolatedStringTextSyntax.TextToken.ValueText);
                            break;
                        case InterpolationSyntax interpolationSyntax:
                            var typeInfo = ModelExtensions.GetTypeInfo(context
                                    .SemanticModel, interpolationSyntax.Expression, token);

                            if (typeInfo.Type is null) return null;

                            interpolations.Add(
                                new InterpolationInfo(
                                    interpolations.Count,
                                    interpolationSyntax.FullSpan.Length,
                                    typeInfo.Type
                                )
                            );
                            // interpolationLengths.Add(interpolationSyntax.FullSpan.Length);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(interpolation));
                    }
                }

                if (content.Count is 0) return null;
                isMultiLine = interpolated.StringStartToken.Kind()
                    is SyntaxKind.MultiLineRawStringLiteralToken
                    or SyntaxKind.InterpolatedMultiLineRawStringStartToken;
                break;


            case LiteralExpressionSyntax {Token.Value: string stringContent} literal:
                content.Add(stringContent);
                isMultiLine = literal.Token.Kind()
                    is SyntaxKind.MultiLineRawStringLiteralToken
                    or SyntaxKind.InterpolatedMultiLineRawStringStartToken;
                break;

            default: return null;
        }


        return new Target(
            location,
            argument.GetLocation(),
            content.ToArray(),
            interpolations.ToArray(),
            isMultiLine,
            context.SemanticModel.Compilation.GetKnownTypes(),
            LookupNode
        );

        ImmutableArray<ISymbol> LookupNode(string? name)
            => context.SemanticModel.LookupNamespacesAndTypes(context.Node.SpanStart, name: name);
    }
}
