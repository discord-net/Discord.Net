using Discord.ComponentDesigner.Generator.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.ComponentDesigner.Generator.Nodes;

public sealed class ComponentNodeContext
{
    public Compilation Compilation => KnownTypes.Compilation;

    public bool HasErrors => _document.HasErrors || Diagnostics.Any(x => x.Severity is DiagnosticSeverity.Error);

    public List<Diagnostic> Diagnostics { get; } = [];

    public KnownTypes KnownTypes { get; }
    public Func<string, ImmutableArray<ISymbol>> LookupNode { get; }

    public readonly InterpolationInfo[] Interpolations;

    private readonly CXmlDoc _document;
    private readonly Location _startLocation;
    private readonly bool _isMultiLine;


    public ComponentNodeContext(
        CXmlDoc document,
        Location startLocation,
        bool isMultiLine,
        InterpolationInfo[] interpolations,
        KnownTypes knownTypes,
        Func<string, ImmutableArray<ISymbol>> lookupNode
    )
    {
        _document = document;
        _startLocation = startLocation;
        _isMultiLine = isMultiLine;
        Interpolations = interpolations;
        KnownTypes = knownTypes;
        LookupNode = lookupNode;
    }

    public void ReportDiagnostic(DiagnosticDescriptor descriptor, Location location, params object?[]? args)
    {
        var diagnostic = Diagnostic.Create(
            descriptor,
            location,
            args
        );

        Diagnostics.Add(diagnostic);
    }

    private int GetInterpolationOffsets(int sourceOffset)
    {
        var result = sourceOffset;
        for (var i = 0; i < _document.InterpolationOffsets.Count; i++)
        {
            if (_document.InterpolationOffsets[i] < sourceOffset)
                result += Interpolations[i].Length;
        }

        return result;
    }

    public Location GetLocation(ICXml node) => GetLocation(node.Span);

    public Location GetLocation(SourceSpan span)
    {
        var sourceSpan = _startLocation.GetLineSpan().Span;

        var startLine = sourceSpan.Start.Line;
        var endLine = startLine;

        if (_isMultiLine)
        {
            startLine += span.Start.Line + 1;
            endLine = startLine + span.LineDelta;
        }

        var startColumn = sourceSpan.Start.Character + span.Start.Column + 1;
        var endColumn = startColumn + span.ColumnDelta;

        var text = _startLocation.SourceTree!.GetText();

        var startTextLine = text.Lines[startLine];

        var startOffset = text.Lines[startLine].Start + startColumn;
        var endOffset = text.Lines[endLine].Start + endColumn;

        startOffset += GetInterpolationOffsets(span.Start.Offset) - span.Start.Offset;
        endOffset += GetInterpolationOffsets(span.End.Offset + 1) - span.End.Offset - 1;

        return _startLocation.SourceTree.GetLocation(new TextSpan(
            startOffset,
            (endOffset - startOffset)
        ));
    }
}
