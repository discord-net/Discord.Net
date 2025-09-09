namespace Discord.ComponentDesignerGenerator.Parser;

public readonly record struct BlendedNode(
    CXNode? Node,
    CXToken? Token,
    CXBlender Blender
);
