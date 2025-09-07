using System.Collections.Generic;
using System.Linq;

namespace Discord.ComponentDesignerGenerator.Parser;

public sealed record CXmlElement(
    SourceSpan Span,
    CXmlValue.Scalar Name,
    IReadOnlyDictionary<string, CXmlAttribute> Attributes,
    IReadOnlyList<ICXml> Children,
    params IReadOnlyList<CXmlDiagnostic> Diagnostics
) : ICXml
{
    public bool HasErrors
        => Diagnostics.Count > 0 ||
           Children.Any(x => x.HasErrors) ||
           Attributes.Values.Any(x => x.HasErrors);

    public CXmlAttribute? GetAttribute(string name)
        => Attributes.TryGetValue(name, out var attribute) ? attribute : null;
}
