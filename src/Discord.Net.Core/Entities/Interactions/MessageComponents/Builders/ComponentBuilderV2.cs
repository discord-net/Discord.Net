using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord;

public class ComponentBuilderV2
{
    public ComponentBuilderV2() {}

    private List<IMessageComponentBuilder> _components = new();

    public List<IMessageComponentBuilder> Components
    {
        get => _components;
        set
        {
            _components = value ?? throw new ArgumentNullException(nameof(value), $"{nameof(Components)} cannot be null.");
        }
    }

    public ComponentBuilderV2 AddComponent(IMessageComponentBuilder component)
    {
        Components.Add(component);
        return this;
    }

    public ComponentBuilderV2 AddComponent(IMessageComponent component)
    {
        return this;
    }

    public ComponentBuilderV2 WithComponents(List<IMessageComponentBuilder> components)
    {
        Components = components;
        return this;
    }

    public ComponentBuilderV2 WithActionRow(ActionRowBuilder actionRow)
    {
        Components.Add(actionRow);
        return this;
    }

    public ComponentBuilderV2 WithTextDisplay(TextDisplayBuilder textDisplayComponent)
    {
        Components.Add(textDisplayComponent);
        return this;
    }

    public ComponentBuilderV2 WithSection(SectionBuilder sectionComponent)
    {
        Components.Add(sectionComponent);
        return this;
    }

    public ComponentBuilderV2 WithMediaGallery(MediaGalleryBuilder mediaGallery)
    {
        Components.Add(mediaGallery);
        return this;
    }

    public ComponentBuilderV2 WithSeparator(SeparatorBuilder separator)
    {
        Components.Add(separator);
        return this;
    }

    public ComponentBuilderV2 WithFile(FileComponentBuilder file)
    {
        Components.Add(file);
        return this;
    }

    public ComponentBuilderV2 WithContainer(ContainerComponentBuilder container)
    {
        Components.Add(container);
        return this;
    }

    public ComponentBuilderV2 WithButton(ButtonBuilder button)
    {
        Components.Add(button);
        return this;
    }

    public ComponentBuilderV2 WithSelectMenu(SelectMenuBuilder selectMenu)
    {
        Components.Add(selectMenu);
        return this;
    }
    public MessageComponent Build()
    {
        return new MessageComponent(Components.Select(x => x.Build()).ToList());
    }
}
