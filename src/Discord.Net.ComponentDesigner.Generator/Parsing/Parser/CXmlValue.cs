using System.Collections.Generic;

namespace Discord.ComponentDesignerGenerator.Parser;

public abstract record CXmlValue(
    SourceSpan Span,
    params IReadOnlyList<CXmlDiagnostic> Diagnostics
) : ICXml
{
    public bool HasErrors => Diagnostics.Count > 0;

    public sealed record Invalid(
        SourceSpan Span,
        params IReadOnlyList<CXmlDiagnostic> Diagnostics
    ) : CXmlValue(Span, Diagnostics);

    public sealed record Scalar(
        SourceSpan Span,
        string Value,
        char? QuoteChar = null,
        params IReadOnlyList<CXmlDiagnostic> Diagnostics
    ) : CXmlValue(Span, Diagnostics);

    public sealed record Interpolation(
        SourceSpan Span,
        int InterpolationIndex,
        params IReadOnlyList<CXmlDiagnostic> Diagnostics
    ) : CXmlValue(Span, Diagnostics);

    public sealed record Multipart(
        SourceSpan Span,
        IReadOnlyList<CXmlValue> Values,
        char? QuoteChar = null,
        params IReadOnlyList<CXmlDiagnostic> Diagnostics
    ) : CXmlValue(Span, Diagnostics);
}
