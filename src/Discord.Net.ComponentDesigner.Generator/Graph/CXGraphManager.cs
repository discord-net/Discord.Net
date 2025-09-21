using Discord.CX.Nodes;
using Discord.CX.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Discord.CX;

public sealed record CXGraphManager(
    SourceGenerator Generator,
    string Key,
    Target Target,
    CXDoc Document
)
{
    public SyntaxTree SyntaxTree => InvocationSyntax.SyntaxTree;
    public InterceptableLocation InterceptLocation => Target.InterceptLocation;
    public InvocationExpressionSyntax InvocationSyntax => Target.InvocationSyntax;
    public ExpressionSyntax ArgumentExpressionSyntax => Target.ArgumentExpressionSyntax;
    public IOperation Operation => Target.Operation;
    public Compilation Compilation => Target.Compilation;

    public string CXDesigner => Target.CXDesigner;
    public DesignerInterpolationInfo[] InterpolationInfos => Target.Interpolations;

    public TextSpan CXDesignerSpan => Target.CXDesignerSpan;

    public CXParser Parser => Document.Parser;

    public CXGraph Graph
    {
        get => _graph ??= CXGraph.Create(this);
        init => _graph = value;
    }

    public string SimpleSource => _simpleSource ??= (
        GetCXWithoutInterpolations(
            CXDesignerSpan.Start,
            CXDesigner,
            InterpolationInfos
        )
    );

    private string? _simpleSource;

    private CXGraph? _graph;

    public CXGraphManager(CXGraphManager other)
    {
        _graph = other.Graph;
        Generator = other.Generator;
        Key = other.Key;
        Target = other.Target;
        Document = other.Document;
    }

    public static CXGraphManager Create(SourceGenerator generator, string key, Target target, CancellationToken token)
    {
        var source = new CXSource(
            target.CXDesignerSpan,
            target.CXDesigner,
            target.Interpolations.Select(x => x.Span).ToArray(),
            target.CXQuoteCount
        );

        var doc = CXParser.Parse(source, token);

        return new CXGraphManager(
            generator,
            key,
            target,
            doc
        );
    }

    public CXGraphManager OnUpdate(string key, Target target, CancellationToken token)
    {
        /*
         * TODO:
         * There are 2 modes of incremental updating: re-parse and re-gen,
         *
         * Reparsing:
         *   This requires incremental parsing and then re-generating the updated nodes that were parsed, we can
         *   re-use old gen information
         *
         * Regenerating
         *   Caused mostly by interpolation types changing, the actual values don't matter since it doesn't change
         *   out emitted code
         *
         *   Some key things to note:
         *     A fast-path is possible for regenerating, if an interpolations content (source code) has changed, we
         *     can skip reparse and regeneration, and simply update any diagnostics' text spans.
         *     If an interpolations type has changed, we re-run the validator wrapping the interpolation, and regenerate
         *     our emitted source.
         */

        var result = this with {Key = key, Target = target};

        var newCXWithoutInterpolations = GetCXWithoutInterpolations(
            target.ArgumentExpressionSyntax.SpanStart,
            target.CXDesigner,
            target.Interpolations
        );

        if (newCXWithoutInterpolations != SimpleSource)
        {
            // we're going to need to reparse, the underlying CX structure changed
            result.DoReparse(target, this, ref result, token);
        }

        return result;
    }

    private void DoReparse(Target target, CXGraphManager old, ref CXGraphManager result, CancellationToken token)
    {
        var source = new CXSource(
            target.CXDesignerSpan,
            target.CXDesigner,
            target.Interpolations.Select(x => x.Span).ToArray(),
            target.CXQuoteCount
        );

        var changes = target
            .SyntaxTree
            .GetChanges(old.SyntaxTree)
            .Where(x => CXDesignerSpan.IntersectsWith(x.Span))
            .ToArray();

        var document = Document.IncrementalParse(
            source,
            changes,
            out var parseResult,
            token
        );

        result = result with
        {
            Graph = result.Graph.Update(result, parseResult, document),
            Document = document
        };
    }

    public RenderedInterceptor Render(CancellationToken token = default)
    {
        var diagnostics = new List<Diagnostic>(
            Document
                .Diagnostics
                .Select(x => Diagnostic.Create(
                        Diagnostics.ParseError,
                        SyntaxTree.GetLocation(x.Span),
                        x.Message
                    )
                )
                .Concat(
                    Graph.Diagnostics
                )
        );

        if (diagnostics.Count > 0)
        {
            return new(InterceptLocation, string.Empty, [..diagnostics]);
        }

        var context = new ComponentContext(Graph) {Diagnostics = diagnostics};

        Graph.Validate(context);

        var source = context.HasErrors
            ? string.Empty
            : Graph.Render(context);

        return new(
            this.InterceptLocation,
            source,
            [..diagnostics]
        );
    }

    private static string GetCXWithoutInterpolations(
        int offset,
        string cx,
        DesignerInterpolationInfo[] interpolations
    )
    {
        if (interpolations.Length is 0) return cx;

        var builder = new StringBuilder(cx);

        var rmDelta = 0;
        for (var i = 0; i < interpolations.Length; i++)
        {
            var interpolation = interpolations[i];
            builder.Remove(interpolation.Span.Start - offset - rmDelta, interpolation.Span.Length);
            rmDelta += interpolation.Span.Length;
        }

        return builder.ToString();
    }
}
