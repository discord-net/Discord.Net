using Discord.CX.Parser;

namespace Discord.CX.Nodes.Components.SelectMenus;

public readonly record struct SelectMenuDefautValue(
    SelectMenuDefaultValueKind Kind,
    CXValue Value
);
