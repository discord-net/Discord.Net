using Microsoft.CodeAnalysis;
using System;

namespace Discord.ComponentDesignerGenerator.Nodes;

[Flags]
public enum NodeKind : int
{
    Unknown = 0,

    ActionRow = 1 << 0,
    Button =  1 << 1,
    StringSelect = 1 << 2,
    TextInput = 1 << 3,
    UserSelect = 1 << 4,
    RoleSelect = 1 << 5,
    MentionableSelect =  1 << 6,
    ChannelSelect = 1 << 7,
    Section = 1 << 8,
    TextDisplay = 1 << 9,
    Thumbnail = 1 << 10,
    MediaGallery = 1 << 11,
    File = 1 << 12,
    Separator = 1 << 13,
    Container = 1 << 14,
    Label = 1 << 15,

    SelectDefault = 1 << 16,
    SelectOption = 1 << 17,
    MediaGalleryItem = 1 << 18,

    SelectMenuMask = UserSelect | RoleSelect | ChannelSelect | MentionableSelect | ChannelSelect | StringSelect,

    AnyComponent = int.MaxValue ^ Interpolated,
    Any = int.MaxValue,

    Custom = 1 << 30,
    Interpolated = 1 << 31
}

public static class NodeFlagExtensions
{
    public static NodeKind ToNodeKind(this ITypeSymbol symbol, KnownTypes types)
    {
        if (types.Compilation.HasImplicitConversion(symbol, types.ActionRowBuilderType))
            return NodeKind.ActionRow;

        if (types.Compilation.HasImplicitConversion(symbol, types.ButtonBuilderType))
            return NodeKind.Button;

        if (types.Compilation.HasImplicitConversion(symbol, types.SelectMenuBuilderType))
            return NodeKind.SelectMenuMask;

        if (types.Compilation.HasImplicitConversion(symbol, types.TextInputBuilderType))
            return NodeKind.TextInput;

        if (types.Compilation.HasImplicitConversion(symbol, types.SectionBuilderType))
            return NodeKind.Section;

        if (types.Compilation.HasImplicitConversion(symbol, types.TextDisplayBuilderType))
            return NodeKind.TextDisplay;

        if (types.Compilation.HasImplicitConversion(symbol, types.ThumbnailBuilderType))
            return NodeKind.Thumbnail;

        if (types.Compilation.HasImplicitConversion(symbol, types.MediaGalleryBuilderType))
            return NodeKind.MediaGallery;

        if (types.Compilation.HasImplicitConversion(symbol, types.FileComponentBuilderType))
            return NodeKind.File;

        if (types.Compilation.HasImplicitConversion(symbol, types.SeparatorBuilderType))
            return NodeKind.Separator;

        if (types.Compilation.HasImplicitConversion(symbol, types.ContainerBuilderType))
            return NodeKind.Container;

        if (types.Compilation.HasImplicitConversion(symbol, types.IMessageComponentBuilderType))
            return NodeKind.AnyComponent;

        return NodeKind.Unknown;
    }

    public static bool IsInterpolated(this NodeKind nodeKind) => nodeKind.HasFlag(NodeKind.Interpolated);

    public static bool IsSelectMenu(this NodeKind nodeKind)
        => (nodeKind & NodeKind.SelectMenuMask) is not 0;
}
