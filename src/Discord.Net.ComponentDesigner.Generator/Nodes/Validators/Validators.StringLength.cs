namespace Discord.ComponentDesigner.Generator.Nodes;

public static partial class Validators
{
    public static ComponentPropertyValidator<string> LengthBounds(
        int? lower = null,
        int? upper = null
    ) => (node, property, context) =>
    {
        if (!property.TryGetScalarValue(out var value)) return;

        if (lower.HasValue && value.Length < lower.Value)
        {
            context.ReportDiagnostic(
                Diagnostics.StringTooShort,
                context.GetLocation(property.Value!),
                property.Name,
                lower.Value
            );
        }

        if (upper.HasValue && value.Length > upper.Value)
        {
            context.ReportDiagnostic(
                Diagnostics.StringTooLong,
                context.GetLocation(property.Value!),
                property.Name,
                upper.Value
            );
        }
    };
}
