using Microsoft.CodeAnalysis;

namespace Discord.ComponentDesignerGenerator;

public static partial class Diagnostics
{
    public static readonly DiagnosticDescriptor ParseError = new(
        "DCP001",
        "CX Parsing error",
        "{}",
        "Component Parser (CX)",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InvalidEnumVariant = new(
        "DC0001",
        "Invalid enum variant",
        "'{0}' is not a valid variant of '{1}'; valid values are '{2}'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor TypeMismatch = new(
        "DC0002",
        "Type mismatch",
        "'{0}' is not of expected type '{1}'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor OutOfRange = new(
        "DC0003",
        "Type mismatch",
        "'{0}' must be {1} in length",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor UnknownComponent = new(
        "DC0004",
        "Unknown component",
        "'{0}' is not a known component",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor ButtonCustomIdUrlConflict = new(
        "DC0005",
        "Invalid button",
        "Buttons cannot contain both a 'url' and a 'customid'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor ButtonCustomIdOrUrlMissing = new(
        "DC0006",
        "Invalid button",
        "A button must specify either a 'customId' or a 'url'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );
}
