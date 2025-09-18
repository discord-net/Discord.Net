namespace Discord.ComponentDesignerGenerator.Parser;

public readonly record struct BlendedNode(
    ICXNode Value,
    CXBlender.Cursor Cursor
);
