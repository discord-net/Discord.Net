using Microsoft.CodeAnalysis.Text;

namespace Discord.ComponentDesignerGenerator.Parser;

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

    public override void IncrementalParse(ParseSlot slot, TextChange change)
    {
        var oldMode = Parser.Lexer.Mode;
        Parser.Lexer.Mode = CXLexer.LexMode.Attribute;

        try
        {
            if (slot == Identifier)
            {
                UpdateSlot(slot, Identifier = Parser.ParseIdentifier());
            }

            // if (slot == EqualsToken)
            // {
            //
            // }
        }
        finally
        {
            Parser.Lexer.Mode = oldMode;
        }

    }
}
