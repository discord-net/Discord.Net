using Discord.ComponentDesignerGenerator.Parser;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Discord.ComponentDesignerGenerator.Nodes;

public sealed class CustomComponent : ComponentNode
{
    private sealed record CustomComponentProp(
        IPropertySymbol Symbol,
        CXmlAttribute? Attribute
    )
    {
        public bool IsOptional => !Symbol.IsRequired;
        public ITypeSymbol PropertyType => Symbol.Type;
    }

    public override string FriendlyName => _symbol.Name;
    public override NodeKind Kind => NodeKind.Custom;

    private readonly ITypeSymbol _symbol;
    private readonly Dictionary<string, CustomComponentProp> _properties;

    public CustomComponent(CXmlElement element, ITypeSymbol symbol, ComponentNodeContext context) : base(element,
        context)
    {
        _symbol = symbol;

        _properties = new Dictionary<string, CustomComponentProp>();

        foreach (var propertySymbol in symbol.GetMembers().OfType<IPropertySymbol>())
        {
            if (propertySymbol.ExplicitInterfaceImplementations.Length is not 0) continue;

            if (propertySymbol.DeclaredAccessibility is not Accessibility.Public) continue;

            var isOptional = !propertySymbol.IsRequired;

            _properties[propertySymbol.Name] = new(
                propertySymbol,
                GetAttribute(propertySymbol.Name)
            );
        }
    }

    private string BuildInstantiation()
    {
        var sb = new StringBuilder($"new {_symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}()");

        var values = new List<string>();

        foreach (var prop in _properties)
        {
            if (prop.Value.Attribute is not null)
            {
                values.Add($"{prop.Key} = {MapPropertyValue(prop.Value, prop.Value.Attribute.Value)}");
            }
        }

        if (values.Count is 0) return sb.ToString();

        sb.AppendLine().AppendLine("{");

        sb.Append(string.Join(",\n".Postfix(4), values.Select(x => x.Prefix(4))));

        sb.AppendLine().Append("}");

        return sb.ToString();
    }

    private string MapPropertyValue(CustomComponentProp prop, CXmlValue? value)
    {
        switch (value)
        {
            case CXmlValue.Interpolation interpolation:
                // easiest case
                var interpolationInfo = Context.Interpolations[interpolation.InterpolationIndex];

                if (!Context.Compilation.HasImplicitConversion(interpolationInfo.Type, prop.PropertyType))
                {
                    Context.ReportDiagnostic(
                        Diagnostics.InvalidAttributeType,
                        Context.GetLocation(interpolation),
                        interpolationInfo.Type.ToDisplayString(),
                        prop.PropertyType.ToDisplayString()
                    );
                }

                return
                    $"designer.GetValue<{prop.PropertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>({interpolation.InterpolationIndex})";

            default:
                return "default";
        }
    }

    public override string Render()
        => $"{BuildInstantiation()}.Render()";
}
