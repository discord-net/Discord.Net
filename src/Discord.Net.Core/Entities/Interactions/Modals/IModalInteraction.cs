using System;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents an interaction type for Modals.
    /// </summary>
    public interface IModalInteraction : IDiscordInteraction
    {
        /// <summary>
        ///     Gets the data received with this interaction; contains the clicked button.
        /// </summary>
        new IModalInteractionData Data { get; }

        /// <summary>
        ///     Gets the message the modal originates from.
        /// </summary>
        /// <remarks>
        ///     This property is only populated if the modal was created from a message component.
        /// </remarks>
        IUserMessage Message { get; }

        /// <summary>
        ///     Updates the message which this modal originates from with the type <see cref="InteractionResponseType.UpdateMessage"/>
        /// </summary>
        /// <param name="func">A delegate containing the properties to modify the message with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>A task that represents the asynchronous operation of updating the message.</returns>
        /// <remarks>
        ///     This method can be used only if the modal was created from a message component.
        /// </remarks>
        Task UpdateAsync(Action<MessageProperties> func, RequestOptions options = null);

        /// <summary>
        ///     Defers an interaction with the response type 5 (<see cref="InteractionResponseType.DeferredChannelMessageWithSource"/>).
        /// </summary>
        /// <param name="ephemeral"><see langword="true"/> to defer ephemerally, otherwise <see langword="false"/>.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>A task that represents the asynchronous operation of acknowledging the interaction.</returns>
        Task DeferLoadingAsync(bool ephemeral = false, RequestOptions options = null);
    }
}
