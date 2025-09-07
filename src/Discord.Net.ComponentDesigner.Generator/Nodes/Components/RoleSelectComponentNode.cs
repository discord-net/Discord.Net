using Discord.ComponentDesignerGenerator.Parser;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesignerGenerator.Nodes;

public sealed class RoleSelectComponentNode : BaseSelectComponentNode
{
    public override string FriendlyName => "Role Select";
    public override NodeKind Kind => NodeKind.RoleSelect;

    public RoleSelectComponentNode(CXmlElement xml, ComponentNodeContext context) : base(xml, context)
    {
    }

    public override string Render()
        => $"""
            new {Context.KnownTypes.SelectMenuBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                customId: {CustomId},
                placeholder: {Placeholder},
                maxValues: {MaxValues},
                minValues: {MinValues},
                isDisabled: {IsDisabled},
                type: {Context.KnownTypes.ComponentTypeEnumType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.RoleSelect,
                id: {Id}
            )
            """;
}
