using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    public static class IDiscordInteractionExtentions
    {
        /// <summary>
        ///     Respond to an interaction with a <see cref="IModal"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="IModal"/> implementation.</typeparam>
        /// <param name="interaction">The interaction to respond to.</param>
        /// <param name="modifyModal">Delegate that can be used to modify the modal.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <returns>A task that represents the asynchronous operation of responding to the interaction.</returns>
        public static async Task RespondWithModalAsync<T>(this IDiscordInteraction interaction, string customId, RequestOptions options = null, Action<ModalBuilder> modifyModal = null)
            where T : class, IModal
        {
            if (!ModalUtils.TryGet<T>(out var modalInfo))
                throw new ArgumentException($"{typeof(T).FullName} isn't referenced by any registered Modal Interaction Command and doesn't have a cached {typeof(ModalInfo)}");

            await SendModalResponseAsync(interaction, customId, modalInfo, options, modifyModal);
        }

        /// <summary>
        ///     Respond to an interaction with a <see cref="IModal"/>.
        /// </summary>
        /// <remarks>
        ///     This method overload uses the <paramref name="interactionService"/> parameter to create a new <see cref="ModalInfo"/>
        ///     if there isn't a built one already in cache.
        /// </remarks>
        /// <typeparam name="T">Type of the <see cref="IModal"/> implementation.</typeparam>
        /// <param name="interaction">The interaction to respond to.</param>
        /// <param name="interactionService">Interaction service instance that should be used to build <see cref="ModalInfo"/>s.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <param name="modifyModal">Delegate that can be used to modify the modal.</param>
        /// <returns></returns>
        public static async Task RespondWithModalAsync<T>(this IDiscordInteraction interaction, string customId, InteractionService interactionService,
            RequestOptions options = null, Action<ModalBuilder> modifyModal = null)
            where T : class, IModal
        {
            var modalInfo = ModalUtils.GetOrAdd<T>(interactionService);

            await SendModalResponseAsync(interaction, customId, modalInfo, options, modifyModal);
        }

        /// <summary>
        ///     Respond to an interaction with an <see cref="IModal"/> and fills the value fields of the modal using the property values of the provided
        ///     instance.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="IModal"/> implementation.</typeparam>
        /// <param name="interaction">The interaction to respond to.</param>
        /// <param name="modal">The <see cref="IModal"/> instance to get field values from.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <param name="modifyModal">Delegate that can be used to modify the modal.</param>
        /// <returns></returns>
        public static async Task RespondWithModalAsync<T>(this IDiscordInteraction interaction, string customId, T modal, RequestOptions options = null,
            Action<ModalBuilder> modifyModal = null)
            where T : class, IModal
        {
            if (!ModalUtils.TryGet<T>(out var modalInfo))
                throw new ArgumentException($"{typeof(T).FullName} isn't referenced by any registered Modal Interaction Command and doesn't have a cached {typeof(ModalInfo)}");

            var builder = new ModalBuilder(modal.Title, customId);

            foreach (var input in modalInfo.Components)
                switch (input)
                {
                    case TextInputComponentInfo textComponent:
                        {
                            builder.AddTextInput(textComponent.Label, textComponent.CustomId, textComponent.Style, textComponent.Placeholder, textComponent.IsRequired ? textComponent.MinLength : null,
                            textComponent.MaxLength, textComponent.IsRequired, textComponent.Getter(modal) as string);
                        }
                        break;
                    default:
                        throw new InvalidOperationException($"{input.GetType().FullName} isn't a valid component info class");
                }

            if (modifyModal is not null)
                modifyModal(builder);

            await interaction.RespondWithModalAsync(builder.Build(), options).ConfigureAwait(false);
        }

        private static async Task SendModalResponseAsync(IDiscordInteraction interaction, string customId, ModalInfo modalInfo, RequestOptions options = null, Action<ModalBuilder> modifyModal = null)
        {
            var builder = new ModalBuilder(modalInfo.Title, customId);

            foreach (var input in modalInfo.Components)
                switch (input)
                {
                    case TextInputComponentInfo textComponent:
                        builder.AddTextInput(textComponent.Label, textComponent.CustomId, textComponent.Style, textComponent.Placeholder, textComponent.IsRequired ? textComponent.MinLength : null,
                            textComponent.MaxLength, textComponent.IsRequired, textComponent.InitialValue);
                        break;
                    default:
                        throw new InvalidOperationException($"{input.GetType().FullName} isn't a valid component info class");
                }

            if (modifyModal is not null)
                modifyModal(builder);

            await interaction.RespondWithModalAsync(builder.Build(), options).ConfigureAwait(false);
        }
    }
}
