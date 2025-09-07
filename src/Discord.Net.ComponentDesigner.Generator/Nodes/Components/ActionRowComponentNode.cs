using Discord.ComponentDesignerGenerator.Parser;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Discord.ComponentDesignerGenerator.Nodes;

public sealed class ActionRowComponentNode : ComponentNode
{
    public override string FriendlyName => "Action Row";

    public override NodeKind Kind => NodeKind.ActionRow;

    public IReadOnlyList<ComponentNode> Components { get; }

    public ActionRowComponentNode(CXmlElement xml, ComponentNodeContext context) : base(xml, context)
    {
        var children = new List<ComponentNode>();

        foreach (var xmlChild in xml.Children)
        {
            ProcessChild(children, xmlChild);
        }

        Components = children;
    }

    private void ProcessChild(List<ComponentNode> children, ICXml child)
    {
        switch (child)
        {
            case CXmlElement element:
                var component = Create(element, Context);

                if (component is null) return;

                if (!IsValidChild(component))
                {
                    Context.ReportDiagnostic(
                        Diagnostics.InvalidChildComponentType,
                        Context.GetLocation(element),
                        FriendlyName,
                        component.FriendlyName
                    );

                    return;
                }

                children.Add(component);
                break;
            case CXmlValue value:

                break;
        }

        void ProcessValue(CXmlValue value)
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
                    var interpolationInfo = Context.Interpolations[interpolation.InterpolationIndex];

                    switch (IsValidChildBuilderType(interpolationInfo.Type))
                    {
                        case true:
                            children.Add(
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
                        ProcessValue(child);
                    }

                    break;
            }
        }
    }

    private bool? IsValidChildBuilderType(ITypeSymbol symbol)
    {
        if (
            !Context.KnownTypes.Compilation.HasImplicitConversion(
                symbol,
                Context.KnownTypes.IMessageComponentBuilderType
            )
        )
        {
            return false;
        }

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
            Context.Compilation.HasImplicitConversion(symbol, Context.KnownTypes.ButtonBuilderType) ||
            Context.Compilation.HasImplicitConversion(symbol, Context.KnownTypes.SelectMenuBuilderType);
    }

    private static bool IsValidChild(ComponentNode node)
        => node is ButtonComponentNode
            or ChannelSelectComponentNode
            or UserSelectComponentNode
            or StringSelectComponentNode
            or RoleSelectComponentNode
            or MentionableSelectComponentNode;

    public override void ReportValidationErrors()
    {
        base.ReportValidationErrors();

        foreach (var component in Components) component.ReportValidationErrors();

        if (Components.Count is 0)
        {
            Context.ReportDiagnostic(
                Diagnostics.EmptyActionRow,
                Location
            );

            return;
        }

        if (Components.Count > Constants.MAX_ACTION_ROW_COMPONENTS)
        {
            Context.ReportDiagnostic(
                Diagnostics.TooManyChildrenInActionRow,
                Location
            );
        }

        if (Components.Count > 1)
        {
            // only multiple buttons are allow
            foreach (var child in Components)
            {
                if (child is ButtonComponentNode) continue;

                Context.ReportDiagnostic(
                    Diagnostics.ActionRowCanOnlyContainMultipleButtons,
                    child.Location,
                    child.FriendlyName
                );
            }
        }
    }

    public override string Render()
        => $"""
            new {Context.KnownTypes.ActionRowBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                {
                    string.Join(
                        ",\n".Postfix(4),
                        Components.Select(x => x.Render().WithNewlinePadding(4))
                    )
                }
            )
            """;
}
