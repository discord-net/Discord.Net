using Discord.ComponentDesignerGenerator.Nodes;
using Discord.ComponentDesignerGenerator.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Discord.ComponentDesignerGenerator;

public sealed class CXGraphManager
{
    public SyntaxTree SyntaxTree => InvocationSyntax.SyntaxTree;
    public InterceptableLocation InterceptLocation => _target.InterceptLocation;
    public InvocationExpressionSyntax InvocationSyntax => _target.InvocationSyntax;
    public ExpressionSyntax ArgumentExpressionSyntax => _target.ArgumentExpressionSyntax;
    public IOperation Operation => _target.Operation;
    public Compilation Compilation => _target.Compilation;

    public string CXDesigner => _target.CXDesigner;
    public DesignerInterpolationInfo[] InterpolationInfos => _target.Interpolations;

    public TextSpan CXDesignerSpan => _target.CXDesignerSpan;

    public CXParser Parser => _document.Parser;

    private readonly SourceGenerator _generator;

    private CXDoc _document;
    private Target _target;
    private string _key;

    private string _basicCXSource;

    private CXGraph _graph;

    public CXGraphManager(
        SourceGenerator generator,
        string key,
        Target target,
        CXDoc document
    )
    {
        _generator = generator;
        _target = target;
        _document = document;
        _key = key;

        _basicCXSource = GetCXWithoutInterpolations(
            CXDesignerSpan.Start,
            CXDesigner,
            InterpolationInfos
        );

        _graph = CXGraph.Create(_document, this);
    }

    public static CXGraphManager Create(SourceGenerator generator, string key, Target target)
    {
        var source = new CXSource(
            target.CXDesignerSpan,
            target.CXDesigner,
            target.Interpolations.Select(x => x.Span).ToArray()
        );

        return new CXGraphManager(generator, key, target, CXParser.Parse(source));
    }

    public void OnUpdate(string key, Target target)
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

        var newCXWithoutInterpolations = GetCXWithoutInterpolations(
            target.ArgumentExpressionSyntax.SpanStart,
            target.CXDesigner,
            target.Interpolations
        );

        if (newCXWithoutInterpolations != _basicCXSource)
        {
            // we're going to need to reparse, the underlying CX structure changed
            DoReparse(target);
        }

        _target = target;
        _key = key;
    }

    private void DoReparse(Target target)
    {
        Debug.Assert(_document is not null);

        var source = new CXSource(
            target.CXDesignerSpan,
            target.CXDesigner,
            target.Interpolations.Select(x => x.Span).ToArray()
        );

        var changes = target
            .SyntaxTree
            .GetChanges(_target.SyntaxTree)
            .Where(x => CXDesignerSpan.Contains(x.Span))
            .ToArray();

        var result = _document!.ApplyChanges(
            source,
            changes
        );

        _graph.Update(_document, result.ReusedNodes);
    }

    public RenderedInterceptor Render()
    {
        var diagnostics = new List<Diagnostic>(
            _document
                .Diagnostics
                .Select(x => Diagnostic.Create(
                        Diagnostics.ParseError,
                        SyntaxTree.GetLocation(x.Span),
                        x.Message
                    )
                )
        );

        if (diagnostics.Count > 0)
        {
            return new(InterceptLocation, string.Empty, [..diagnostics]);
        }

        var context = new ComponentContext(_graph) {Diagnostics = diagnostics};

        _graph.Validate(context);

        var source = context.HasErrors
            ? string.Empty
            : _graph.Render(context);

        return new(
            this.InterceptLocation,
            _graph.Render(),
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

        for (var i = 0; i < interpolations.Length; i++)
        {
            var interpolation = interpolations[i];
            builder.Remove(interpolation.Span.Start - offset, interpolation.Span.Length);
        }

        return builder.ToString();
    }
}
