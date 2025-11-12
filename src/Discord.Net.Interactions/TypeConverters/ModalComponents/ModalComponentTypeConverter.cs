using System;
using System.Threading.Tasks;

namespace Discord.Interactions;

/// <summary>
///     Base class for creating ModalComponentTypeConverters. <see cref="InteractionService"/> uses ModalComponentTypeConverters to interface with Modal component parameters.
/// </summary>
public abstract class ModalComponentTypeConverter : ITypeConverter<IComponentInteractionData>
{
    /// <summary>
    ///     Will be used to search for alternative ModalComponentTypeConverters whenever the Interaction Service encounters an unknown parameter type.
    /// </summary>
    /// <param name="type">Type of the modal property.</param>
    /// <returns>Whether this converter can be used to handle the given type.</returns>
    public abstract bool CanConvertTo(Type type);

    /// <summary>
    ///     Will be used to read the incoming payload before building the modal instance.
    /// </summary>
    /// <param name="context">Command execution context.</param>
    /// <param name="option">Received option payload.</param>
    /// <param name="services">Service provider that will be used to initialize the command module.</param>
    /// <returns>The result of the read process.</returns>
    public abstract Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services);

    /// <summary>
    ///     Will be used to manipulate the outgoing modal component, before the modal gets sent to Discord.
    /// </summary>
    public virtual Task WriteAsync<TBuilder>(TBuilder builder, IDiscordInteraction interaction, InputComponentInfo component, object value)
        where TBuilder : class, IInteractableComponentBuilder
        => Task.CompletedTask;

    protected bool TryGetModalInteractionData(IInteractionContext context, out IModalInteractionData modalData)
    {
        if(context.Interaction is IModalInteraction modalInteraction)
        {
            modalData = modalInteraction.Data;
            return true;
        }

        modalData = null;
        return false;
    }
}

/// <inheritdoc/>
public abstract class ModalComponentTypeConverter<T> : ModalComponentTypeConverter
{
    /// <inheritdoc/>
    public sealed override bool CanConvertTo(Type type) =>
        typeof(T).IsAssignableFrom(type);
}
