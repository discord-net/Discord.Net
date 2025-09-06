using Discord.ComponentDesigner.Generator.Parser;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Discord.ComponentDesigner.Generator.Nodes;

public sealed class ContainerComponentNode : ComponentNode
{
    public override string FriendlyName => "Container";
    public override NodeKind Kind => NodeKind.Container;
    public ComponentProperty<string> AccentColor { get; }

    public ComponentProperty<bool> IsSpoiler { get; }

    public IReadOnlyList<ComponentNode> Components { get; }

    public ContainerComponentNode(CXmlElement xml, ComponentNodeContext context) : base(xml, context)
    {
        AccentColor = MapProperty(
            "accentColor",
            optional: true,
            parser: ValueParsers.ParseColorProperty,
            aliases: ["color"]
        );

        IsSpoiler = MapProperty<bool>(
            "spoiler",
            ValueParsers.ParseBooleanProperty,
            optional: true
        );

        var components = new List<ComponentNode>();

        foreach (var childXml in xml.Children)
        {
            switch (childXml)
            {
                case CXmlValue value:
                    ExtractChildrenFromXmlElementValue(components, value);
                    break;
                case CXmlElement element:
                    var component = Create(element, context);

                    if (component is null) continue;

                    if (!IsValidChildType(component.Kind))
                    {
                        context.ReportDiagnostic(
                            Diagnostics.InvalidChildComponentType,
                            component.Location,
                            FriendlyName,
                            component.FriendlyName
                        );

                        continue;
                    }

                    components.Add(component);
                    break;
                default:
                    context.ReportDiagnostic(
                        Diagnostics.InvalidChildNodeType,
                        context.GetLocation(childXml),
                        FriendlyName,
                        "text"
                    );
                    break;
            }
        }

        Components = components;
    }

    private static bool IsValidChildType(NodeKind kind)
        => (
            kind & (NodeKind.Custom | NodeKind.ActionRow | NodeKind.TextDisplay | NodeKind.Section |
                    NodeKind.MediaGallery | NodeKind.Separator | NodeKind.File)
        ) is not 0;

    private void ExtractChildrenFromXmlElementValue(
        List<ComponentNode> components,
        CXmlValue value
    )
    {
        switch (value)
        {
            case CXmlValue.Scalar scalar:
                if (string.IsNullOrWhiteSpace(scalar.Value)) break;

                Context.ReportDiagnostic(
                    Diagnostics.InvalidChildNodeType,
                    Context.GetLocation(scalar),
                    FriendlyName,
                    "text"
                );
                break;

            case CXmlValue.Interpolation interpolation:
                // verify it's a component
                var interpolationInfo = Context.Interpolations[interpolation.InterpolationIndex];

                switch (IsValidChildType(interpolationInfo.Type))
                {
                    case true:
                        components.Add(
                            new InterpolatedComponentNode(
                                interpolation,
                                Context,
                                Context.KnownTypes.IMessageComponentBuilderType!.ToString()
                            )
                        );
                        break;
                    case false:
                        Context.ReportDiagnostic(
                            Diagnostics.InvalidChildNodeType,
                            Context.GetLocation(interpolation),
                            FriendlyName,
                            interpolationInfo.Type.ToDisplayString()
                        );
                        return;
                    case null:
                        Context.ReportDiagnostic(
                            Diagnostics.PossibleInvalidChildNodeType,
                            Context.GetLocation(interpolation),
                            interpolationInfo.Type.ToDisplayString(),
                            FriendlyName
                        );
                        return;
                }

                break;
            case CXmlValue.Multipart multipart:
                foreach (var child in multipart.Values)
                {
                    ExtractChildrenFromXmlElementValue(components, child);
                }

                break;
        }
    }

    private bool? IsValidChildType(ITypeSymbol symbol)
    {
        // ensure it inherits the component builder
        if (
            !Context.KnownTypes.Compilation.HasImplicitConversion(
                symbol,
                Context.KnownTypes.IMessageComponentBuilderType
            )
        )
        {
            return false;
        }

        // if it is just a builder, it may not be a valid type
        if (
            Context
                .KnownTypes
                .IMessageComponentBuilderType?
                .Equals(
                    symbol,
                    SymbolEqualityComparer.Default
                ) ?? false
        )
        {
            return null;
        }

        return
            Context.Compilation.HasImplicitConversion(symbol, Context.KnownTypes.ActionRowBuilderType) ||
            Context.Compilation.HasImplicitConversion(symbol, Context.KnownTypes.TextDisplayBuilderType) ||
            Context.Compilation.HasImplicitConversion(symbol, Context.KnownTypes.SectionBuilderType) ||
            Context.Compilation.HasImplicitConversion(symbol, Context.KnownTypes.MediaGalleryBuilderType) ||
            Context.Compilation.HasImplicitConversion(symbol, Context.KnownTypes.SeparatorBuilderType) ||
            Context.Compilation.HasImplicitConversion(symbol, Context.KnownTypes.FileComponentBuilderType);
    }

    public override void ReportValidationErrors()
    {
        base.ReportValidationErrors();

        foreach (var component in Components) component.ReportValidationErrors();
    }

    public override string Render()
        => $"""
            new {Context.KnownTypes.ContainerBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                {
                    string.Join(
                        ",\n".Postfix(4),
                        Components.Select(x => x.Render().WithNewlinePadding(4))
                    )
                }
            )
            """;
}
