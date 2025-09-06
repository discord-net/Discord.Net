using Discord.ComponentDesigner.Generator.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesigner.Generator.Nodes;

public static class ValueCodeGenerator
{
    public static string? BuildValue(CXmlValue? value, ComponentNodeContext context)
    {
        switch (value)
        {
            case CXmlValue.Invalid or null: return null;

            case CXmlValue.Interpolation interpolation:
                var type = context.Interpolations[interpolation.InterpolationIndex].Type;
                return $"designer.GetValue<{type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>({interpolation.InterpolationIndex})";

            case CXmlValue.Multipart multipart:
                return BuildMultipart(multipart);
            case CXmlValue.Scalar scalar:
                var sb = new StringBuilder();

                // escape out the double quotes
                var quoteCount = scalar.Value.Count(x => x is '"') + 1;


                var isMultiLine = scalar.Value.Contains("\n");

                if (isMultiLine)
                {
                    sb.AppendLine();
                    quoteCount = Math.Max(3, quoteCount);
                }

                var quotes = new string('"', quoteCount);

                sb.Append(quotes);

                if (isMultiLine) sb.AppendLine();

                sb.Append(ComponentProperty<string>.FixValuePadding(scalar.Span.Start.Column, scalar.Value));

                if (isMultiLine) sb.AppendLine();

                sb.Append(quotes);

                return sb.ToString();
            default:
                throw new ArgumentOutOfRangeException(nameof(value));
        }
    }

    public static string BuildMultipart(CXmlValue.Multipart value)
        => BuildMultipart(value.Values);

    public static string BuildMultipart(IReadOnlyList<CXmlValue> values)
    {
        if (values.Count is 0) return string.Empty;

        // count how many dollar signs we need
        var bracketCount = 0;
        var quoteCount = 0;
        var isMultiLine = false;

        foreach (var part in values)
        {
            if (part is not CXmlValue.Scalar scalar) continue;

            isMultiLine |= scalar.Value.Contains("\n");

            var localCount = Math.Max(
                scalar.Value.Count(x => x is '{'),
                scalar.Value.Count(x => x is '}')
            );

            bracketCount = Math.Max(bracketCount, localCount);
            quoteCount = Math.Max(quoteCount, scalar.Value.Count(x => x is '"'));
        }

        var dollarSignCount = bracketCount + 1;
        quoteCount++;

        if (isMultiLine)
            quoteCount = Math.Max(quoteCount, 3);

        var sb = new StringBuilder();

        var quotes = new string('"', quoteCount);
        var startInterp = new string('{', dollarSignCount);
        var endInterp = new string('}', dollarSignCount);


        foreach (var part in values)
        {
            switch (part)
            {
                case CXmlValue.Scalar scalar:
                    sb.Append(scalar.Value);
                    break;
                case CXmlValue.Interpolation interpolation:
                    sb.Append(startInterp)
                        .Append("designer.GetValueAsString(")
                        .Append(interpolation.InterpolationIndex)
                        .Append(')')
                        .Append(endInterp);
                    break;
            }
        }

        var content = FixValuePadding(values[0].Span.Start.Column, sb.ToString());

        sb.Clear();

        if (isMultiLine) sb.AppendLine();

        sb.Append(new string('$', dollarSignCount)).Append(quotes);


        if (isMultiLine) sb.AppendLine();

        sb.Append(content);

        if (isMultiLine) sb.AppendLine();

        return sb.Append(quotes).ToString();
    }

    public static string FixValuePadding(int startingPad, string value)
    {
        // find the min padding between each line
        var split = value.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
        var paddings = new int[split.Length];

        var min = startingPad;

        for (var i = 1; i < split.Length; i++)
        {
            var line = split[i];

            if (string.IsNullOrWhiteSpace(line)) continue;

            var lineIndex = 0;
            for (; lineIndex < line.Length && char.IsWhiteSpace(line[lineIndex]); lineIndex++) ;

            min = Math.Min(min, lineIndex);
        }

        // // remove useless previous lines
        // for (var i = split.Length - 1; i >= 0; i--)
        // {
        //     if (string.IsNullOrWhiteSpace(split[i])) split[i] = string.Empty;
        //
        //     break;
        // }

        var result = string.Join(
            "\n",
            [
                split[0],
                ..split.Skip(1)
                    .Select(x => x.Length > min ? x.Substring(min) : x)
            ]
        );

        return result.Trim(['\n', '\r', ' ', '\t']);
    }
}
