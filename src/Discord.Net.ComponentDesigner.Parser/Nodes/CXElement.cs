using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;

namespace Discord.CX.Parser;

public sealed class CXElement : CXNode
{
    public string Identifier => ElementStartNameToken.Value;

    public CXToken ElementStartOpenToken { get; }
    public CXToken ElementStartNameToken { get; }
    public CXCollection<CXAttribute> Attributes { get; }

    public CXToken ElementStartCloseToken { get; }

    public CXCollection<CXNode> Children { get; }

    public CXToken? ElementEndOpenToken { get; }
    public CXToken? ElementEndNameToken { get; }
    public CXToken? ElementEndCloseToken { get; }

    public CXElement(
        CXToken elementStartOpenToken,
        CXToken elementStartNameToken,
        CXCollection<CXAttribute> attributes,
        CXToken elementStartCloseToken,
        CXCollection<CXNode> children,
        CXToken? elementEndOpenToken = null,
        CXToken? elementEndNameToken = null,
        CXToken? elementEndCloseToken = null
    )
    {
        Slot(ElementStartOpenToken = elementStartOpenToken);
        Slot(ElementStartNameToken = elementStartNameToken);
        Slot(Attributes = attributes);
        Slot(ElementStartCloseToken = elementStartCloseToken);
        Slot(Children = children);
        Slot(ElementEndOpenToken = elementEndOpenToken);
        Slot(ElementEndNameToken = elementEndNameToken);
        Slot(ElementEndCloseToken = elementEndCloseToken);
    }
}
