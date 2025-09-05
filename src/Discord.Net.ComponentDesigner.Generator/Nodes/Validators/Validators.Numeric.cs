namespace Discord.ComponentDesigner.Generator.Nodes;

partial class Validators
{
    public static ComponentPropertyValidator<int> Bounds(
        int? lower,
        int? upper
    ) => (node, property, context) =>
    {
        if (!property.TryGetScalarValue(out var value)) return;

        if (!int.TryParse(value, out var number)) return;

        if (number < lower)
        {
            context.ReportDiagnostic(
                Diagnostics.StringTooShort,
                context.GetLocation(property.Value!),
                property.Name,
                lower.Value
            );
        }

        if (number > upper)
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
