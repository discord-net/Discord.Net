using Discord.ComponentDesigner.Generator.Parser;
using System.Collections.Generic;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesigner.Generator.Nodes;

public sealed class StringSelectComponentNode : BaseSelectComponentNode
{
    public override string FriendlyName => "String Select";
    public override NodeKind Kind => NodeKind.StringSelect;
    public IReadOnlyList<SelectOption> Options { get; }

    public StringSelectComponentNode(CXmlElement xml, ComponentNodeContext context) : base(
        xml,
        context,
        hasDefaultValues: false
    )
    {
        var options = new List<SelectOption>();

        foreach (var childXml in xml.Children)
        {
            if (childXml is not CXmlElement element)
            {
                context.ReportDiagnostic(
                    Diagnostics.InvalidChildNodeType,
                    context.GetLocation(childXml),
                    FriendlyName,
                    "text"
                );

                continue;
            }

            if (element.Name.Value is not "option")
            {
                context.ReportDiagnostic(
                    Diagnostics.InvalidChildComponentType,
                    context.GetLocation(childXml),
                    FriendlyName,
                    element.Name.Value
                );

                continue;
            }

            options.Add(new SelectOption(element, context));
        }

        Options = options;
    }

    public override void ReportValidationErrors()
    {
        base.ReportValidationErrors();

        foreach (var option in Options) option.ReportValidationErrors();
    }

    public override string Render()
        => $"""
            new {Context.KnownTypes.SelectMenuBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                customId: {CustomId.ToString().WithNewlinePadding(4)},
                placeholder: {Placeholder.ToString().WithNewlinePadding(4)},
                maxValues: {MaxValues.ToString().WithNewlinePadding(4)},
                minValues: {MinValues.ToString().WithNewlinePadding(4)},
                isDisabled: {IsDisabled.ToString().WithNewlinePadding(4)},
                type: {Context.KnownTypes.ComponentTypeEnumType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.StringSelect
            )
            """;
}
