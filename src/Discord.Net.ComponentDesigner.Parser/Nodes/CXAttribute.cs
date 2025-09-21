using Microsoft.CodeAnalysis.Text;

namespace Discord.CX.Parser;

public sealed class CXAttribute : CXNode
{
    public CXToken Identifier { get; private set; }

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
}
