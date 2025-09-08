using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

namespace Discord.ComponentDesignerGenerator.Parser;

public sealed class CXElement : CXNode
{
    public CXToken ElementStartOpenToken { get; }
    public CXToken ElementStartNameToken { get; }
    public IReadOnlyList<CXAttribute> Attributes { get; }

    public CXToken ElementStartCloseToken { get; }

    public IReadOnlyList<CXNode> Children { get; }

    public CXToken? ElementEndOpenToken { get; }
    public CXToken? ElementEndNameToken { get; }
    public CXToken? ElementEndCloseToken { get; }

    public CXElement(
        CXToken elementStartOpenToken,
        CXToken elementStartNameToken,
        IReadOnlyList<CXAttribute> attributes,
        CXToken elementStartCloseToken,
        IEnumerable<CXNode>? children = null,
        CXToken? elementEndOpenToken = null,
        CXToken? elementEndNameToken = null,
        CXToken? elementEndCloseToken = null
    )
    {
        Slot(ElementStartOpenToken = elementStartOpenToken);
        Slot(ElementStartNameToken = elementStartNameToken);
        Slot(Attributes = attributes);
        Slot(ElementStartCloseToken = elementStartCloseToken);
        Slot(Children = [..children ?? []]);
        Slot(ElementEndOpenToken = elementEndOpenToken);
        Slot(ElementEndNameToken = elementEndNameToken);
        Slot(ElementEndCloseToken = elementEndCloseToken);
    }

    public override void IncrementalParse(ParseSlot slot, TextChange change)
    {

    }
}
