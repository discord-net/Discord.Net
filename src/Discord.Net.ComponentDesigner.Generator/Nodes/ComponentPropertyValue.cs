using Discord.ComponentDesignerGenerator.Parser;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Discord.ComponentDesignerGenerator.Nodes;

public sealed record ComponentPropertyValue(
    ComponentProperty Property,
    CXAttribute? Attribute
)
{
    public CXValue? Value => Attribute?.Value;

    public bool IsSpecified => Attribute is not null;

    private readonly List<Diagnostic> _diagnostics = [];

    public void AddDiagnostic(Diagnostic diagnostic) => _diagnostics.Add(diagnostic);
}
