using System.Collections.Generic;

namespace Discord.ComponentDesignerGenerator.Parser;

public interface ICXml
{
    SourceSpan Span { get; }

    IReadOnlyList<CXmlDiagnostic> Diagnostics { get; }

    bool HasErrors { get; }
}
