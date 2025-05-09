using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord;

public static class ComponentBuilderExtensions
{
    /// <summary>
    ///     Sets the custom id for the component.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public static BuilderT WithId<BuilderT>(this BuilderT builder, int? id)
        where BuilderT : IMessageComponentBuilder
    {
        builder.Id = id;
        return builder;
    }

    /// <summary>
    ///     Converts a <see cref="IMessageComponent"/> to a builder.
    /// </summary>
    /// <exception cref="ArgumentException">Unknown component type</exception>
    public static IMessageComponentBuilder ToBuilder(this IMessageComponent component)
        => component switch
        {
            ActionRowComponent actionRow => actionRow.ToBuilder(),
            ButtonComponent button => button.ToBuilder(),
            SelectMenuComponent select => select.ToBuilder(),
            SectionComponent section => section.ToBuilder(),
            TextDisplayComponent textDisplay => textDisplay.ToBuilder(),
            ThumbnailComponent thumbnail => thumbnail.ToBuilder(),
            MediaGalleryComponent mediaGallery => mediaGallery.ToBuilder(),
            FileComponent file => file.ToBuilder(),
            SeparatorComponent separator => separator.ToBuilder(),
            ContainerComponent container => container.ToBuilder(),
            _ => throw new ArgumentException("Unknown component type")
        };

    /// <summary>
    ///     Converts a <see cref="FileComponent"/> to a <see cref="FileComponentBuilder"/>.
    /// </summary>
    public static FileComponentBuilder ToBuilder(this FileComponent file)
        => new(file);

    /// <summary>
    ///     Converts a <see cref="SeparatorComponent"/> to a <see cref="SeparatorBuilder"/>.
    /// </summary>
    public static SeparatorBuilder ToBuilder(this SeparatorComponent separator)
        => new(separator);

    /// <summary>
    ///     Converts a <see cref="MediaGalleryComponent"/> to a <see cref="MediaGalleryBuilder"/>.
    /// </summary>
    public static MediaGalleryBuilder ToBuilder(this MediaGalleryComponent mediaGallery)
        => new(mediaGallery);

    /// <summary>
    ///     Converts a <see cref="ButtonComponent"/> to a <see cref="ButtonBuilder"/>.
    /// </summary>
    public static ButtonBuilder ToBuilder(this ButtonComponent button)
        => new(button);

    /// <summary>
    ///     Converts a <see cref="SelectMenuComponent"/> to a <see cref="SelectMenuBuilder"/>.
    /// </summary>
    public static SelectMenuBuilder ToBuilder(this SelectMenuComponent select)
        => new(select);

    /// <summary>
    ///     Converts a <see cref="ActionRowComponent"/> to a <see cref="ActionRowBuilder"/>.
    /// </summary>
    public static ActionRowBuilder ToBuilder(this ActionRowComponent actionRow)
        => new(actionRow);

    /// <summary>
    ///     Converts a <see cref="ContainerComponent"/> to a <see cref="ContainerBuilder"/>.
    /// </summary>
    public static ContainerBuilder ToBuilder(this ContainerComponent container)
        => new(container);

    /// <summary>
    ///     Converts a <see cref="SectionComponent"/> to a <see cref="SectionBuilder"/>.
    /// </summary>
    public static SectionBuilder ToBuilder(this SectionComponent section)
        => new(section);

    /// <summary>
    ///     Converts a <see cref="ThumbnailComponent"/> to a <see cref="ThumbnailBuilder"/>.
    /// </summary>
    public static ThumbnailBuilder ToBuilder(this ThumbnailComponent thumbnail)
        => new(thumbnail);

    /// <summary>
    ///     Converts a <see cref="TextDisplayComponent"/> to a <see cref="TextDisplayBuilder"/>.
    /// </summary>
    public static TextDisplayBuilder ToBuilder(this TextDisplayComponent textDisplay)
        => new (textDisplay);

    /// <summary>
    ///     Converts a <see cref="MediaGalleryItem"/> to a <see cref="MediaGalleryItemProperties"/>.
    /// </summary>
    public static MediaGalleryItemProperties ToProperties(this MediaGalleryItem item)
        => new(item.Media.ToProperties(), item.Description, item.IsSpoiler);

    /// <summary>
    ///     Converts a <see cref="UnfurledMediaItem"/> to a <see cref="UnfurledMediaItemProperties"/>.
    /// </summary>
    public static UnfurledMediaItemProperties ToProperties(this UnfurledMediaItem item)
        => new(item.Url);

    /// <summary>
    ///     Converts a collection of <see cref="IMessageComponent"/> to a <see cref="ComponentBuilderV2"/>.
    /// </summary>
    public static ComponentBuilderV2 ToBuilder(this IEnumerable<IMessageComponent> components)
        => new(components);
}
