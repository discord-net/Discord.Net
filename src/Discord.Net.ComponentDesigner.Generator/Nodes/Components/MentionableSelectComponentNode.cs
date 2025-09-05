using Discord.ComponentDesigner.Generator.Parser;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesigner.Generator.Nodes;

public sealed class MentionableSelectComponentNode : BaseSelectComponentNode
{
    public override string FriendlyName => "Mentionable Select";

    public override NodeKind Kind => NodeKind.MentionableSelect;

    public MentionableSelectComponentNode(CXmlElement xml, ComponentNodeContext context) : base(xml, context)
    {
    }

    public override string Render()
        => $"""
            new {Context.KnownTypes.SelectMenuBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                customId: {CustomId.ToString().WithNewlinePadding(4)},
                placeholder: {Placeholder.ToString().WithNewlinePadding(4)},
                maxValues: {MaxValues.ToString().WithNewlinePadding(4)},
                minValues: {MinValues.ToString().WithNewlinePadding(4)},
                isDisabled: {IsDisabled.ToString().WithNewlinePadding(4)},
                type: {Context.KnownTypes.ComponentTypeEnumType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.MentionableSelect,
                id: {Id}
            )
            """;
}
