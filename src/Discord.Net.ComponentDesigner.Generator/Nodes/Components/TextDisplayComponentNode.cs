using Discord.ComponentDesigner.Generator.Parser;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesigner.Generator.Nodes;

public sealed class TextDisplayComponentNode : ComponentNode
{
    public override string FriendlyName => "Text Display";
    public override NodeKind Kind => NodeKind.TextDisplay;
    public CXmlValue? Value { get; }

    public TextDisplayComponentNode(CXmlElement xml, ComponentNodeContext context) : base(xml, context)
    {
        if (xml.Children.Count is 0) return;

        if (xml.Children.Count > 1 || xml.Children[0] is not CXmlValue value)
        {
            context.ReportDiagnostic(
                Diagnostics.TextCannotContainComponents,
                Location
            );

            return;
        }

        Value = value;
    }

    private string RenderContent() => ComponentProperty<string>.BuildValue(Value) ?? "default";

    public override string Render()
        => $"""
            new {Context.KnownTypes.TextDisplayBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                content: {RenderContent().WithNewlinePadding(4)},
                id: {Id}
            )
            """;
}
