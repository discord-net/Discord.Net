namespace Discord;

public interface IMessageComponentBuilder
{
    /// <summary>
    ///     Gets the type of the component.
    /// </summary>
    ComponentType Type { get; }

    /// <summary>
    ///     Gets or sets the id for the component. An autoincremented id will be assigned if not set.
    /// </summary>
    int? Id { get; set;  }

    /// <summary>
    ///     Runs validation checks and builds the component.
    /// </summary>
    IMessageComponent Build();
}
