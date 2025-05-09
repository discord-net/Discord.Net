using System.Collections.Generic;
using System.Linq;

namespace Discord;

public static class ComponentContainerExtensions
{
    /// <summary>
    ///     Gets the total number of components in this and all child <see cref="IComponentContainer"/>s combined.
    /// </summary>
    public static int ComponentCount(this IComponentContainer container)
        => (container.Components?.Count ?? 0)
        + container.Components?
            .OfType<IComponentContainer>()
            .Sum(x => x.ComponentCount()) ?? 0;

    /// <summary>
    ///     Adds a <see cref="TextDisplayBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithTextDisplay<BuilderT>(this BuilderT container, TextDisplayBuilder textDisplay)
        where BuilderT : class, IStaticComponentContainer
    {
        container.AddComponent(textDisplay);
        return container;
    }

    /// <summary>
    ///     Adds a <see cref="TextDisplayBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithTextDisplay<BuilderT>(this BuilderT container,
        string content,
        int? id = null)
        where BuilderT : class, IStaticComponentContainer
        => container.WithTextDisplay(new TextDisplayBuilder()
            .WithContent(content)
            .WithId(id));

    /// <summary>
    ///     Adds a <see cref="SectionBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithSection<BuilderT>(this BuilderT container, SectionBuilder section)
        where BuilderT : class, IStaticComponentContainer
    {
        container.AddComponent(section);
        return container;
    }

    /// <summary>
    ///     Adds a <see cref="SectionBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithSection<BuilderT>(this BuilderT container,
        IEnumerable<TextDisplayBuilder> components,
        IMessageComponentBuilder accessory,
        bool isSpoiler = false,
        int? id = null)
        where BuilderT : class, IStaticComponentContainer
        => container.WithSection(new SectionBuilder()
            .WithComponents(components)
            .WithAccessory(accessory)
            .WithId(id));

    /// <summary>
    ///     Adds a <see cref="MediaGalleryBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithMediaGallery<BuilderT>(this BuilderT container, MediaGalleryBuilder mediaGallery)
        where BuilderT : class, IStaticComponentContainer
    {
        container.AddComponent(mediaGallery);
        return container;
    }

    /// <summary>
    ///     Adds a <see cref="MediaGalleryBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithMediaGallery<BuilderT>(this BuilderT container,
        IEnumerable<MediaGalleryItemProperties> items,
        int? id = null) where BuilderT : class, IStaticComponentContainer
        => container.WithMediaGallery(new MediaGalleryBuilder()
            .WithItems(items)
            .WithId(id));

    /// <summary>
    ///     Adds a <see cref="MediaGalleryBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithMediaGallery<BuilderT>(this BuilderT container,
        IEnumerable<string> urls,
        int? id = null)
        where BuilderT : class, IStaticComponentContainer
        => container.WithMediaGallery(new MediaGalleryBuilder()
            .WithItems(urls.Select(x => new MediaGalleryItemProperties(new UnfurledMediaItemProperties(x))))
            .WithId(id));

    /// <summary>
    ///     Adds a <see cref="SeparatorBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithSeparator<BuilderT>(this BuilderT container, SeparatorBuilder separator)
        where BuilderT : class, IStaticComponentContainer
    {
        container.AddComponent(separator);
        return container;
    }

    /// <summary>
    ///     Adds a <see cref="SeparatorBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithSeparator<BuilderT>(this BuilderT container,
        SeparatorSpacingSize spacing = SeparatorSpacingSize.Small,
        bool isDivider = true,
        int? id = null)
        where BuilderT : class, IStaticComponentContainer
        => container.WithSeparator(new SeparatorBuilder()
            .WithSpacing(spacing)
            .WithIsDivider(isDivider)
            .WithId(id));

    /// <summary>
    ///     Adds a <see cref="FileComponentBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithFile<BuilderT>(this BuilderT container, FileComponentBuilder file)
        where BuilderT : class, IStaticComponentContainer
    {
        container.AddComponent(file);
        return container;
    }

    /// <summary>
    ///     Adds a <see cref="FileComponentBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithFile<BuilderT>(this BuilderT container,
        UnfurledMediaItemProperties file,
        bool isSpoiler = false,
        int? id = null)
        where BuilderT : class, IStaticComponentContainer
        => container.WithFile(new FileComponentBuilder()
            .WithFile(file)
            .WithIsSpoiler(isSpoiler)
            .WithId(id));

    /// <summary>
    ///     Adds a <see cref="ContainerBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithContainer<BuilderT>(this BuilderT container, ContainerBuilder containerComponent)
        where BuilderT : class, IStaticComponentContainer
    {
        container.AddComponent(containerComponent);
        return container;
    }

    /// <summary>
    ///     Adds a <see cref="ContainerBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithContainer<BuilderT>(this BuilderT container,
        IEnumerable<IMessageComponentBuilder> components,
        Color? accentColor = null,
        bool isSpoiler = false,
        int? id = null)
        where BuilderT : class, IStaticComponentContainer
        => container.WithContainer(new ContainerBuilder()
            .WithComponents(components)
            .WithAccentColor(accentColor)
            .WithSpoiler(isSpoiler)
            .WithId(id));

    /// <summary>
    ///     Adds a <see cref="ContainerBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithContainer<BuilderT>(this BuilderT container,
        params IMessageComponentBuilder[] components)
        where BuilderT : class, IStaticComponentContainer
        => container.WithContainer(new ContainerBuilder()
            .WithComponents(components));

    /// <summary>
    ///     Adds a <see cref="ButtonBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithButton<BuilderT>(this BuilderT container, ButtonBuilder button)
        where BuilderT : class, IInteractableComponentContainer
    {
        container.AddComponent(button);
        return container;
    }

    /// <summary>
    ///     Adds a <see cref="ButtonBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithButton<BuilderT>(this BuilderT container,
        string label = null,
        string customId = null,
        ButtonStyle style = ButtonStyle.Primary,
        IEmote emote = null,
        string url = null,
        bool disabled = false,
        ulong? skuId = null,
        int? id = null)
        where BuilderT : class, IInteractableComponentContainer
    => container.WithButton(new ButtonBuilder()
            .WithLabel(label)
            .WithStyle(style)
            .WithEmote(emote)
            .WithCustomId(customId)
            .WithUrl(url)
            .WithDisabled(disabled)
            .WithSkuId(skuId)
            .WithId(id));

    /// <summary>
    ///     Adds a <see cref="SelectMenuBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithSelectMenu<BuilderT>(this BuilderT container, SelectMenuBuilder selectMenu)
        where BuilderT : class, IInteractableComponentContainer
    {
        container.AddComponent(selectMenu);
        return container;
    }

    /// <summary>
    ///     Adds a <see cref="SelectMenuBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithSelectMenu<BuilderT>(this BuilderT container,
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
        where BuilderT : class, IInteractableComponentContainer
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

    /// <summary>
    ///     Adds a <see cref="ActionRowBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithActionRow<BuilderT>(this BuilderT container, ActionRowBuilder actionRow)
        where BuilderT : class, IStaticComponentContainer
    {
        container.AddComponent(actionRow);
        return container;
    }

    /// <summary>
    ///     Adds a <see cref="ActionRowBuilder"/> to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    public static BuilderT WithActionRow<BuilderT>(this BuilderT container,
        IEnumerable<IMessageComponentBuilder> components,
        int? id = null)
        where BuilderT : class, IStaticComponentContainer
        => container.WithActionRow(new ActionRowBuilder()
            .WithComponents(components)
            .WithId(id));

    /// <summary>
    ///     Finds the first <see cref="IMessageComponentBuilder"/> in the <see cref="IComponentContainer"/>
    ///     or any of its child <see cref="IComponentContainer"/>s with matching id.
    /// </summary>
    /// <returns>
    ///     The <see cref="IMessageComponentBuilder"/> with matching id, <see langword="null"/> otherwise.
    /// </returns>
    public static IMessageComponentBuilder FindComponentById(this IComponentContainer container, int id)
        => container.FindComponentById<IMessageComponentBuilder>(id);

    /// <summary>
    ///     Finds the first <c>ComponentT</c> in the <see cref="IComponentContainer"/>
    ///     or any of its child <see cref="IComponentContainer"/>s with matching id.
    /// </summary>
    /// <returns>
    ///     The <c>ComponentT</c> with matching id, <see langword="null"/> otherwise.
    /// </returns>
    public static ComponentT FindComponentById<ComponentT>(this IComponentContainer container, int id)
        where ComponentT : class, IMessageComponentBuilder
    {
        if (container is ComponentT cmp && cmp.Id == id)
            return cmp;

        foreach (var component in container.Components)
        {
            if (component.Id == id && component is ComponentT target)
                return target;

            if (component is SectionBuilder section
                && section.Accessory.Id == id
                && section.Accessory is ComponentT targetAccessory)
                return targetAccessory;

            if (component is IComponentContainer childContainer)
            {
                var childSearchResult = childContainer.FindComponentById<ComponentT>(id);
                if (childSearchResult is not null)
                    return childSearchResult;
            }
        }

        return null;
    }

    /// <summary>
    ///     Gets a <see cref="IEnumerable{T}">IEnumerable</see> containing ids of <see cref="IMessageComponentBuilder"/>
    ///     in this <see cref="IComponentContainer"/> and all child <see cref="IComponentContainer"/>s.
    /// </summary>
    public static IEnumerable<int> GetComponentIds(this IComponentContainer container)
        => container.Components
            .Where(x => x.Id is not null)
            .Select(x => x.Id.Value)
            .Concat(container.Components
                .OfType<IComponentContainer>()
                .SelectMany(x => x.GetComponentIds()));

    /// <summary>
    ///     Finds the first <see cref="IMessageComponent"/> in the <see cref="INestedComponent"/>
    ///     or any of its child <see cref="INestedComponent"/>s with matching id.
    /// </summary>
    /// <returns>
    ///     The <see cref="IMessageComponent"/> with matching id, <see langword="null"/> otherwise.
    /// </returns>
    public static IMessageComponent FindComponentById(this INestedComponent container, int id)
        => container.FindComponentById<IMessageComponent>(id);

    /// <summary>
    ///     Finds the first <c>ComponentT</c> in the <see cref="INestedComponent"/>
    ///     or any of its child <see cref="INestedComponent"/>s with matching id.
    /// </summary>
    /// <returns>
    ///     The <c>ComponentT</c> with matching id, <see langword="null"/> otherwise.
    /// </returns>
    public static ComponentT FindComponentById<ComponentT>(this INestedComponent container, int id)
        where ComponentT : class, IMessageComponent
    {
        if (container is ComponentT cmp && cmp.Id == id)
            return cmp;

        return container.Components.FindComponentById<ComponentT>(id);
    }

    /// <summary>
    ///     Finds the first <c>ComponentT</c> in the <see cref="IEnumerable{IMessageComponent}"/>
    ///     or any of its child <see cref="INestedComponent"/>s with matching id.
    /// </summary>
    /// <returns>
    ///     The <c>ComponentT</c> with matching id, <see langword="null"/> otherwise.
    /// </returns>
    public static ComponentT FindComponentById<ComponentT>(this IEnumerable<IMessageComponent> components, int id)
        where ComponentT : class, IMessageComponent
    {
        foreach (var component in components)
        {
            if (component.Id == id && component is ComponentT target)
                return target;

            if (component is SectionComponent section
                && section.Accessory.Id == id
                && section.Accessory is ComponentT targetAccessory)
                return targetAccessory;

            if (component is INestedComponent childContainer)
            {
                var childSearchResult = childContainer.FindComponentById<ComponentT>(id);
                if (childSearchResult is not null)
                    return childSearchResult;
            }
        }

        return null;
    }

    /// <summary>
    ///     Finds the first <see cref="IMessageComponent"/> in the <see cref="IEnumerable{IMessageComponent}"/>
    ///     or any of its child <see cref="INestedComponent"/>s with matching id.
    /// </summary>
    /// <returns>
    ///     The <see cref="IMessageComponent"/> with matching id, <see langword="null"/> otherwise.
    /// </returns>
    public static IMessageComponent FindComponentById(this IEnumerable<IMessageComponent> components, int id)
        => components.FindComponentById<IMessageComponent>(id);
}
