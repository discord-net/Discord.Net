using Microsoft.CodeAnalysis.Text;

namespace Discord.ComponentDesignerGenerator.Parser;

public sealed class CXAttribute : CXNode
{
    public CXToken Identifier { get; }

    public CXToken? EqualsToken { get; }

    public CXValue? Value { get; }

    public CXAttribute(
        CXToken identifier,
        CXToken? equalsToken,
        CXValue? value
    )
    {
        Slot(Identifier = identifier);
        Slot(EqualsToken = equalsToken);
        Slot(Value = value);
    }

    public override void IncrementalParse(ParseSlot slot, TextChange change)
    {
        if (slot == Identifier)
        {

        }
    }
}
