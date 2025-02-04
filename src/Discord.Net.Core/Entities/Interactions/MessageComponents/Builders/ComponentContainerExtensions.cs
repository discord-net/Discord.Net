using System.Collections.Generic;
using System.Linq;

namespace Discord;

public static class ComponentContainerExtensions
{
    public static IStaticComponentContainer WithTextDisplay(this IStaticComponentContainer container, TextDisplayBuilder textDisplay)
    {
        container.AddComponent(textDisplay);
        return container;
    }
    public static IStaticComponentContainer WithTextDisplay(this IStaticComponentContainer container,
        string content,
        int? id = null)
        => container.WithTextDisplay(new TextDisplayBuilder()
            .WithContent(content)
            .WithId(id));

    public static IStaticComponentContainer WithSection(this IStaticComponentContainer container, SectionBuilder section)
    {
        container.AddComponent(section);
        return container;
    }

    public static IStaticComponentContainer WithSection(this IStaticComponentContainer container,
        IEnumerable<TextDisplayBuilder> components,
        IMessageComponentBuilder accessory,
        bool isSpoiler = false,
        int? id = null)
        => container.WithSection(new SectionBuilder()
            .WithComponents(components)
            .WithAccessory(accessory)
            .WithId(id));

    public static IStaticComponentContainer WithMediaGallery(this IStaticComponentContainer container, MediaGalleryBuilder mediaGallery)
    {
        container.AddComponent(mediaGallery);
        return container;
    }

    public static IStaticComponentContainer WithMediaGallery(this IStaticComponentContainer container,
        IEnumerable<MediaGalleryItemProperties> items,
        int? id = null)
        => container.WithMediaGallery(new MediaGalleryBuilder()
            .WithItems(items)
            .WithId(id));

    public static IStaticComponentContainer WithMediaGallery(this IStaticComponentContainer container,
        IEnumerable<string> urls,
        int? id = null)
        => container.WithMediaGallery(new MediaGalleryBuilder()
            .WithItems(urls.Select(x => new MediaGalleryItemProperties(new UnfurledMediaItemProperties(x))))
            .WithId(id));

    public static IStaticComponentContainer WithSeparator(this IStaticComponentContainer container, SeparatorBuilder separator)
    {
        container.AddComponent(separator);
        return container;
    }

    public static IStaticComponentContainer WithSeparator(this IStaticComponentContainer container,
        SeparatorSpacingSize spacing = SeparatorSpacingSize.Small,
        bool isDivider = true,
        int? id = null)
        => container.WithSeparator(new SeparatorBuilder()
            .WithSpacing(spacing)
            .WithIsDivider(isDivider)
            .WithId(id));

    public static IStaticComponentContainer WithFile(this IStaticComponentContainer container, FileComponentBuilder file)
    {
        container.AddComponent(file);
        return container;
    }

    public static IStaticComponentContainer WithFile(this IStaticComponentContainer container,
        string url,
        bool isSpoiler = false,
        int? id = null)
        => container.WithFile(new FileComponentBuilder()
            .WithFile(new UnfurledMediaItemProperties(url))
            .WithIsSpoiler(isSpoiler)
            .WithId(id));

    public static IStaticComponentContainer WithContainer(this IStaticComponentContainer container, ContainerComponentBuilder containerComponent)
    {
        container.AddComponent(containerComponent);
        return container;
    }

    public static IStaticComponentContainer WithContainer(this IStaticComponentContainer container,
        IEnumerable<IMessageComponentBuilder> components,
        Color? accentColor = null,
        bool isSpoiler = false,
        int? id = null)
        => container.WithContainer(new ContainerComponentBuilder()
            .WithComponents(components)
            .WithAccentColor(accentColor)
            .WithSpoiler(isSpoiler)
            .WithId(id));

    public static IInteractableComponentContainer WithButton(this IInteractableComponentContainer container, ButtonBuilder button)
    {
        container.AddComponent(button);
        return container;
    }

    public static IInteractableComponentContainer WithButton(this IInteractableComponentContainer container,
        string label = null,
        string customId = null,
        ButtonStyle style = ButtonStyle.Primary,
        IEmote emote = null,
        string url = null,
        bool disabled = false,
        ulong? skuId = null,
        int? id = null)
    => container.WithButton(new ButtonBuilder()
            .WithLabel(label)
            .WithStyle(style)
            .WithEmote(emote)
            .WithCustomId(customId)
            .WithUrl(url)
            .WithDisabled(disabled)
            .WithSkuId(skuId)
            .WithId(id));

    public static IInteractableComponentContainer WithSelectMenu(this IInteractableComponentContainer container, SelectMenuBuilder selectMenu)
    {
        container.AddComponent(selectMenu);
        return container;
    }

    public static IInteractableComponentContainer WithSelectMenu(this IInteractableComponentContainer container,
        string customId,
        List<SelectMenuOptionBuilder> options = null,
        string placeholder = null,
        int minValues = 1,
        int maxValues = 1,
        bool disabled = false,
        int row = 0,
        ComponentType type = ComponentType.SelectMenu,
        ChannelType[] channelTypes = null,
        SelectMenuDefaultValue[] defaultValues = null,
        int? id = null)
        => container.WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId(customId)
                .WithOptions(options)
                .WithPlaceholder(placeholder)
                .WithMaxValues(maxValues)
                .WithMinValues(minValues)
                .WithDisabled(disabled)
                .WithType(type)
                .WithChannelTypes(channelTypes)
                .WithDefaultValues(defaultValues)
                .WithId(id));

    public static IStaticComponentContainer WithActionRow(this IStaticComponentContainer container, ActionRowBuilder actionRow)
    {
        container.AddComponent(actionRow);
        return container;
    }

    public static IStaticComponentContainer WithActionRow(this IStaticComponentContainer container,
        IEnumerable<IMessageComponentBuilder> components,
        int? id = null)
        => container.WithActionRow(new ActionRowBuilder()
            .WithComponents(components)
            .WithId(id));
}
