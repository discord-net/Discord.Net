using Discord.ComponentDesigner.Generator.Parser;

namespace Discord.ComponentDesigner.Generator.Nodes;

public sealed class TextInputComponentNode : ComponentNode
{
    public override string FriendlyName => "Text Input";
    public override NodeKind Kind => NodeKind.TextInput;
    public TextInputComponentNode(CXmlElement xml, ComponentNodeContext context) : base(xml, context)
    {
    }

    public override string Render()
    {
        throw new System.NotImplementedException();
    }
}

public enum TextInputStyle
{
    Snort = 1,
    Paragraph = 2
}
