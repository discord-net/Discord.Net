using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Discord.ComponentDesignerGenerator.Parser;

public readonly record struct CXDiagnostic(
    DiagnosticSeverity Severity,
    string Message,
    TextSpan Span
);
