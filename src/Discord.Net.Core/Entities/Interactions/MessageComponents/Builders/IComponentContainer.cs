using System.Collections.Generic;

namespace Discord;

public interface IComponentContainer
{
    List<IMessageComponentBuilder> Components { get; }

    IComponentContainer AddComponent(IMessageComponentBuilder component);

    IComponentContainer AddComponents(params IEnumerable<IMessageComponentBuilder> components);

    IComponentContainer WithComponents(IEnumerable<IMessageComponentBuilder> components);
}
