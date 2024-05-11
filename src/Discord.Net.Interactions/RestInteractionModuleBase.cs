using Discord.Rest;
using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Provides a base class for a Rest based command module to inherit from.
    /// </summary>
    /// <typeparam name="T">Type of interaction context to be injected into the module.</typeparam>
    public abstract class RestInteractionModuleBase<T> : InteractionModuleBase<T>
        where T : class, IInteractionContext
    {
        /// <summary>
        ///     Gets or sets the underlying Interaction Service.
        /// </summary>
        public InteractionService InteractionService { get; set; }

        /// <summary>
        ///     Defer a Rest based Discord Interaction using the <see cref="InteractionServiceConfig.RestResponseCallback"/> delegate.
        /// </summary>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="options">The request options for this response.</param>
        /// <returns>
        ///     A Task representing the operation of creating the interaction response.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if the interaction isn't a type of <see cref="RestInteraction"/>.</exception>
        protected override Task DeferAsync(bool ephemeral = false, RequestOptions options = null)
            => HandleInteractionAsync(x => x.Defer(ephemeral, options));

        /// <summary>
        ///     Respond to a Rest based Discord Interaction using the <see cref="InteractionServiceConfig.RestResponseCallback"/> delegate.
        /// </summary>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="embeds">A array of embeds to send with this response. Max 10.</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="components">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <returns>
        ///     A Task representing the operation of creating the interaction response.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if the interaction isn't a type of <see cref="RestInteraction"/>.</exception>
        protected override Task RespondAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null, Embed embed = null, PollProperties poll = null)
            => HandleInteractionAsync(x => x.Respond(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll));

        /// <summary>
        ///     Responds to the interaction with a modal.
        /// </summary>
        /// <param name="modal">The modal to respond with.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <returns>
        ///     A Task representing the operation of creating the interaction response.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if the interaction isn't a type of <see cref="RestInteraction"/>.</exception>
        protected override Task RespondWithModalAsync(Modal modal, RequestOptions options = null)
            => HandleInteractionAsync(x => x.RespondWithModal(modal, options));

        /// <summary>
        ///     Responds to the interaction with a modal.
        /// </summary>
        /// <typeparam name="TModal">The type of the modal.</typeparam>
        /// <param name="customId">The custom ID of the modal.</param>
        /// <param name="modal">The modal to respond with.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <param name="modifyModal">Delegate that can be used to modify the modal.</param>
        /// <returns>
        ///     A Task representing the operation of creating the interaction response.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if the interaction isn't a type of <see cref="RestInteraction"/>.</exception>
        protected override async Task RespondWithModalAsync<TModal>(string customId, TModal modal, RequestOptions options = null, Action<ModalBuilder> modifyModal = null)
            => await HandleInteractionAsync(x => x.RespondWithModal(customId, modal, options, modifyModal));

        /// <summary>
        ///     Responds to the interaction with a modal.
        /// </summary>
        /// <typeparam name="TModal"></typeparam>
        /// <param name="customId">The custom ID of the modal.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <param name="modifyModal">Delegate that can be used to modify the modal.</param>
        /// <returns>
        ///     A Task representing the operation of creating the interaction response.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if the interaction isn't a type of <see cref="RestInteraction"/>.</exception>
        protected override Task RespondWithModalAsync<TModal>(string customId, RequestOptions options = null, Action<ModalBuilder> modifyModal = null) 
            => HandleInteractionAsync(x => x.RespondWithModal<TModal>(customId, options, modifyModal));

        private Task HandleInteractionAsync(Func<RestInteraction, string> action)
        {
            if (Context.Interaction is not RestInteraction restInteraction)
                throw new InvalidOperationException($"Interaction must be a type of {nameof(RestInteraction)} in order to execute this method.");

            var payload = action(restInteraction);

            if (Context is IRestInteractionContext restContext && restContext.InteractionResponseCallback != null)
                return restContext.InteractionResponseCallback.Invoke(payload);
            else
                return InteractionService._restResponseCallback(Context, payload);
        }
    }
}
