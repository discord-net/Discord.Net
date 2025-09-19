using Discord.ComponentDesignerGenerator.Parser;

namespace Discord.ComponentDesignerGenerator.Nodes.Components.SelectMenus;

public readonly record struct SelectMenuDefautValue(
    SelectMenuDefaultValueKind Kind,
    CXValue Value
);
