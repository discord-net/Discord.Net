using System.Collections.Generic;

namespace Discord.ComponentDesigner.Generator.Parser;

public sealed record CXmlAttribute(
    SourceSpan Span,
    CXmlValue.Scalar Name,
    SourceSpan NameSpan,
    CXmlValue? Value,
    params IReadOnlyList<CXmlDiagnostic> Diagnostics
) : ICXml
{
    public bool HasErrors => Diagnostics.Count > 0 || (Value?.HasErrors ?? false);
}
