using Discord.CX.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;

namespace Discord.CX.Nodes;

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

    public string GetDesignerValue(CXValue.Interpolation interpolation, string? type = null)
        => GetDesignerValue(interpolation.InterpolationIndex, type);

    public string GetDesignerValue(DesignerInterpolationInfo interpolation, string? type = null)
        => GetDesignerValue(interpolation.Id, type);

    public string GetDesignerValue(int index, string? type = null)
        => type is not null ? $"designer.GetValue<{type}>({index})" : $"designer.GetValueAsString({index})";


    public Location GetLocation(ICXNode node)
        => _graph.GetLocation(node);
    public Location GetLocation(TextSpan span)
        => _graph.GetLocation(span);

    public void AddDiagnostic(DiagnosticDescriptor descriptor, ICXNode node, params object?[]? args)
        => AddDiagnostic(Diagnostic.Create(descriptor, GetLocation(node), args));

    public void AddDiagnostic(DiagnosticDescriptor descriptor, TextSpan span, params object?[]? args)
        => AddDiagnostic(Diagnostic.Create(descriptor, GetLocation(span), args));


    public DesignerInterpolationInfo GetInterpolationInfo(CXValue.Interpolation interpolation)
        => GetInterpolationInfo(interpolation.InterpolationIndex);

    public DesignerInterpolationInfo GetInterpolationInfo(int index) => _graph.Manager.InterpolationInfos[index];

    public void AddDiagnostic(Diagnostic diagnostics)
    {
        Diagnostics.Add(diagnostics);
    }
}
