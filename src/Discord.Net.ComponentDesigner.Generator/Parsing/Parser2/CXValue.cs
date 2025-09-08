using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

namespace Discord.ComponentDesignerGenerator.Parser;

public abstract class CXValue : CXNode
{
    public override void IncrementalParse(ParseSlot slot, TextChange change)
    {

    }

    public sealed class Invalid : CXValue;

    public sealed class StringLiteral : CXValue
    {
        public CXToken StartToken { get; }
        public IReadOnlyList<CXToken> Tokens { get; }
        public CXToken EndToken { get; }

        public StringLiteral(
            CXToken start,
            List<CXToken> tokens,
            CXToken end
        )
        {
            Slot(StartToken = start);
            Slot(Tokens = tokens);
            Slot(EndToken = end);
        }
    }

    public sealed class Interpolation : CXValue
    {
        public CXToken Token { get; }
        public int InterpolationIndex { get; }

        public Interpolation(CXToken token, int interpolationIndex)
        {
            Slot(Token = token);
            InterpolationIndex = interpolationIndex;
        }
    }

    public sealed class Scalar : CXValue
    {
        public string Value => Document.GetTokenValue(Token);
        public CXToken Token { get; }

        public Scalar(CXToken token)
        {
            Slot(Token = token);
        }
    }
}
