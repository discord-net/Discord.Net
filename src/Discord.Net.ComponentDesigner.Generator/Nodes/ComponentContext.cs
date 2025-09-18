using Discord.ComponentDesignerGenerator.Parser;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Discord.ComponentDesignerGenerator.Nodes;

public sealed class ComponentContext
{
    public KnownTypes KnownTypes => Compilation.GetKnownTypes();
    public Compilation Compilation => _graph.Manager.Compilation;

    public bool HasErrors => Diagnostics.Any(x => x.Severity is DiagnosticSeverity.Error);

    public List<Diagnostic> Diagnostics { get; init; } = [];

    private readonly CXGraph _graph;

    public ComponentContext(CXGraph graph)
    {
        _graph = graph;
    }

    public Location GetLocation(ICXNode node)
        => _graph.Manager.SyntaxTree.GetLocation(node.Span);

    public void AddDiagnostic(DiagnosticDescriptor descriptor, ICXNode node, params object?[]? args)
        => AddDiagnostic(Diagnostic.Create(descriptor, GetLocation(node), args));


    public DesignerInterpolationInfo GetInterpolationInfo(CXValue.Interpolation interpolation)
        => GetInterpolationInfo(interpolation.InterpolationIndex);

    public DesignerInterpolationInfo GetInterpolationInfo(int index) => _graph.Manager.InterpolationInfos[index];

    public void AddDiagnostic(Diagnostic diagnostics)
    {
        Diagnostics.Add(diagnostics);
    }
}
