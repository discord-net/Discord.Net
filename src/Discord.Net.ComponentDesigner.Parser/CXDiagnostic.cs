using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Discord.CX.Parser;

public readonly record struct CXDiagnostic(
    DiagnosticSeverity Severity,
    string Message,
    TextSpan Span
);
