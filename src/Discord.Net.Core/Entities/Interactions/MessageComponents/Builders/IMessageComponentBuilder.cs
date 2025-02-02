namespace Discord;

public interface IMessageComponentBuilder
{
    ComponentType Type { get; }

    int? Id { get; set;  }

    IMessageComponent Build();
}
