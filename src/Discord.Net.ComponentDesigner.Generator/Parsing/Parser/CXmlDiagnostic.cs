using Microsoft.CodeAnalysis;

namespace Discord.ComponentDesignerGenerator.Parser;

public readonly record struct CXmlDiagnostic(
    DiagnosticSeverity Severity,
    string Message,
    SourceSpan Span
);
