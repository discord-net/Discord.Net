using Discord.ComponentDesigner.Generator.Parser;
using System.Collections.Generic;
using System.Linq;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesigner.Generator.Nodes;

public sealed class SectionComponentNode : ComponentNode
{
    public override string FriendlyName => "Section";

    public override NodeKind Kind => NodeKind.Section;

    public ComponentNode? Accessory { get; }

    public IReadOnlyList<ComponentNode> Components { get; }

    public SectionComponentNode(CXmlElement xml, ComponentNodeContext context, bool mapId = true) : base(xml, context,
        mapId)
    {
        var components = new List<ComponentNode>();
        ComponentNode? accessory = null;

        foreach (var childXml in xml.Children)
        {
            ProcessChildren(components, childXml, ref accessory);
        }

        Accessory = accessory;
        Components = components;
    }

    private void ProcessChildren(List<ComponentNode> children, ICXml xml, ref ComponentNode? accessory)
    {
        switch (xml)
        {
            case CXmlElement element:
                if (element.Name.Value is "accessory")
                {
                    if (accessory is not null)
                    {
                        Context.ReportDiagnostic(
                            Diagnostics.ExtraAccessory,
                            Context.GetLocation(element)
                        );

                        return;
                    }

                    switch (element.Children.Count)
                    {
                        case 0:
                            Context.ReportDiagnostic(
                                Diagnostics.MissingAccessory,
                                Context.GetLocation(element)
                            );
                            break;
                        case > 1:
                            var head = element.Children[1];
                            var tail = element.Children.Last();

                            Context.ReportDiagnostic(
                                Diagnostics.ExtraAccessory,
                                Context.GetLocation((head.Span.Start, tail.Span.End))
                            );

                            break;
                        default:
                            accessory = Create(element.Children[0], Context);
                            if (accessory is null) return;

                            if (!IsValidAccessoryComponent(accessory.Kind))
                            {
                                Context.ReportDiagnostic(
                                    Diagnostics.InvalidChildComponentType,
                                    accessory.Location,
                                    "accessory",
                                    accessory.FriendlyName
                                );
                            }

                            break;
                    }

                    return;
                }

                var component = Create(element, Context);

                if (component is null) return;

                if (!IsValidChildComponent(component.Kind))
                {
                    Context.ReportDiagnostic(
                        Diagnostics.InvalidChildComponentType,
                        component.Location,
                        "accessory",
                        component.FriendlyName
                    );

                    return;
                }

                children.Add(component);
                break;

            case CXmlValue value:
                ProcessValue(value);
                break;
        }

        void ProcessValue(CXmlValue value)
        {
            switch (value)
            {
                case CXmlValue.Scalar scalar:
                    if (string.IsNullOrWhiteSpace(scalar.Value)) return;

                    Context.ReportDiagnostic(
                        Diagnostics.InvalidChildNodeType,
                        Context.GetLocation(scalar),
                        FriendlyName,
                        "text"
                    );
                    return;
                case CXmlValue.Interpolation interpolation:
                    var interpolationInfo = Context.Interpolations[interpolation.InterpolationIndex];

                    var kind = interpolationInfo.Type.ToNodeKind(Context.KnownTypes);

                    if (
                        kind is NodeKind.Unknown ||
                        !Context.KnownTypes.Compilation.HasImplicitConversion(
                            interpolationInfo.Type,
                            Context.KnownTypes.IMessageComponentBuilderType
                        ) ||
                        !IsValidChildComponent(kind)
                    )
                    {
                        Context.ReportDiagnostic(
                            Diagnostics.InvalidChildNodeType,
                            Context.GetLocation(interpolation),
                            FriendlyName,
                            interpolationInfo.Type.ToDisplayString()
                        );
                        return;
                    }

                    if (kind is NodeKind.AnyComponent)
                    {
                        Context.ReportDiagnostic(
                            Diagnostics.PossibleInvalidChildNodeType,
                            Context.GetLocation(interpolation),
                            interpolationInfo.Type.ToDisplayString(),
                            FriendlyName
                        );
                    }

                    children.Add(
                        new InterpolatedComponentNode(
                            interpolation,
                            Context,
                            Context.KnownTypes.IMessageComponentBuilderType!.ToString()
                        )
                    );

                    return;
                case CXmlValue.Multipart multipart:
                    foreach (var part in multipart.Values)
                    {
                        ProcessValue(part);
                    }

                    return;
            }
        }
    }

    private static bool IsValidAccessoryComponent(NodeKind kind)
        => kind.HasFlag(NodeKind.Button) || kind.HasFlag(NodeKind.Thumbnail);

    private static bool IsValidChildComponent(NodeKind kind)
        => kind.HasFlag(NodeKind.TextDisplay);

    public override void ReportValidationErrors()
    {
        base.ReportValidationErrors();

        Accessory?.ReportValidationErrors();
        foreach (var component in Components) component.ReportValidationErrors();

        if (Accessory is null)
        {
            Context.ReportDiagnostic(
                Diagnostics.MissingAccessory,
                Location
            );
        }

        if (
            Accessory is not null and not ButtonComponentNode and not ThumbnailComponentNode
        )
        {
            Context.ReportDiagnostic(
                Diagnostics.InvalidChildComponentType,
                Accessory.Location,
                "accessory",
                Accessory.FriendlyName
            );
        }

        if (Components.Count is 0)
        {
            Context.ReportDiagnostic(
                Diagnostics.MissingSectionComponents,
                Location
            );
        }

        if (Components.Count > Constants.MAX_SECTION_CHILDREN)
        {
            Context.ReportDiagnostic(
                Diagnostics.TooManySectionComponentChildren,
                Location
            );
        }
    }

    public override string Render()
        => $"""
            new {Context.KnownTypes.SectionBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                accessory: {Accessory?.Render().WithNewlinePadding(4) ?? "null"},
                components:
                [
                    {
                        string.Join(
                            ",\n".Postfix(8),
                            Components.Select(x => x.Render().WithNewlinePadding(8))
                        )
                    }
                ]
            )
            """;
}
