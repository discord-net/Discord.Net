using Discord.ComponentDesignerGenerator.Parser;
using System.Collections.Generic;

namespace Discord.ComponentDesignerGenerator.Nodes;

public interface IComponentProperty
{
    string Name { get; }
    bool IsSpecified { get; }

    CXmlAttribute? Attribute { get; }
    CXmlValue? Value { get; }

    IReadOnlyList<string> Aliases { get; }

    void Validate(ComponentNodeContext context);
}
