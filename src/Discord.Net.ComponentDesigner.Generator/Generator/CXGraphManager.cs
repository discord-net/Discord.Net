using Discord.ComponentDesignerGenerator.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Discord.ComponentDesignerGenerator;

public sealed class CXGraphManager
{
    public InterceptableLocation InterceptLocation => _target.InterceptLocation;
    public InvocationExpressionSyntax InvocationSyntax => _target.InvocationSyntax;
    public ExpressionSyntax ArgumentExpressionSyntax => _target.ArgumentExpressionSyntax;
    public IOperation Operation => _target.Operation;
    public Compilation Compilation => _target.Compilation;

    public string CXDesigner => _target.CXDesigner;
    public DesignerInterpolationInfo[] InterpolationInfos => _target.Interpolations;

    public TextSpan CXDesignerSpan => _target.CXDesignerSpan;

    public CXParser Parser => _document.Parser;

    private readonly SourceManager _manager;

    private CXDoc _document;
    private Target _target;
    private string _key;

    private string _basicCXSource;

    public CXGraphManager(
        SourceManager manager,
        string key,
        Target target,
        CXDoc document
    )
    {
        _manager = manager;
        _target = target;
        _document = document;
        _key = key;

        _basicCXSource = GetCXWithoutInterpolations(
            CXDesignerSpan.Start,
            CXDesigner,
            InterpolationInfos
        );
    }

    public static CXGraphManager Create(SourceManager manager, string key, Target target)
    {
        var source = new CXSource(
            target.CXDesignerSpan,
            target.CXDesigner,
            target.Interpolations.Select(x => x.Span).ToArray()
        );

        return new CXGraphManager(manager, key, target, CXParser.Parse(source));
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
         */

        var newSource = GetCXWithoutInterpolations(
            target.ArgumentExpressionSyntax.SpanStart,
            target.CXDesigner,
            target.Interpolations
        );

        if (newSource != _basicCXSource)
        {
            // we're gonna need to reparse
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

        _document!.ApplyChanges(
            source,
            [
                ..target
                    .SyntaxTree
                    .GetChanges(_target.SyntaxTree)
                    .Where(x => CXDesignerSpan.Contains(x.Span))
            ]
        );
    }

    private static string GetCXWithoutInterpolations(int offset, string cx, DesignerInterpolationInfo[] interpolations)
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
