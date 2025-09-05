using Microsoft.CodeAnalysis;

namespace Discord.ComponentDesigner.Generator.Parser;

public readonly record struct CXmlDiagnostic(
    DiagnosticSeverity Severity,
    string Message,
    SourceSpan Span
);
