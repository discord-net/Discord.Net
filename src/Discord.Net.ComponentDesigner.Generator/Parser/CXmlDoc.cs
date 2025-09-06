using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Discord.ComponentDesigner.Generator.Parser;

public sealed record CXmlDoc(
    SourceSpan Span,
    IReadOnlyList<CXmlElement> Elements,
    IReadOnlyList<int> InterpolationOffsets,
    params IReadOnlyList<CXmlDiagnostic> Diagnostics
) : ICXml
{
    public bool HasErrors =>
        Diagnostics.Any(x => x.Severity is DiagnosticSeverity.Error) || Elements.Any(x => x.HasErrors);
}
