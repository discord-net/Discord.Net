using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord;

public class ComponentBuilderV2 : IStaticComponentContainer
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

    public ComponentBuilderV2 AddComponents(params IEnumerable<IMessageComponentBuilder> components)
    {
        foreach (var component in components)
            Components.Add(component);
        return this;
    }

    public ComponentBuilderV2 WithComponents(IEnumerable<IMessageComponentBuilder> components)
    {
        Components = components.ToList();
        return this;
    }

    public MessageComponent Build()
    {
        return new MessageComponent(Components.Select(x => x.Build()).ToList());
    }

    IComponentContainer IComponentContainer.AddComponent(IMessageComponentBuilder component) => AddComponent(component);
    IComponentContainer IComponentContainer.AddComponents(params IEnumerable<IMessageComponentBuilder> components) => AddComponents(components);
    IComponentContainer IComponentContainer.WithComponents(IEnumerable<IMessageComponentBuilder> components) => WithComponents(components);
}
