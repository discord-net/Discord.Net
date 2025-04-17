using Discord.Interactions;
using System;

namespace Discord.Rest
{
    public static class RestExtensions
    {
        /// <summary>
        ///     Respond to an interaction with a <see cref="IModal"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="IModal"/> implementation.</typeparam>
        /// <param name="interaction">The interaction to respond to.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <returns>Serialized payload to be used to create a HTTP response.</returns>
        public static string RespondWithModal<T>(this RestInteraction interaction, string customId, RequestOptions options = null, Action<ModalBuilder> modifyModal = null)
            where T : class, IModal
        {
            if (!ModalUtils.TryGet<T>(out var modalInfo))
                throw new ArgumentException($"{typeof(T).FullName} isn't referenced by any registered Modal Interaction Command and doesn't have a cached {typeof(ModalInfo)}");

            var modal = modalInfo.ToModal(customId, modifyModal);
            return interaction.RespondWithModal(modal, options);
        }

        /// <summary>
        ///     Respond to an interaction with an <see cref="IModal"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="IModal"/> implementation.</typeparam>
        /// <param name="interaction">The interaction to respond to.</param>
        /// <param name="modal">The <see cref="IModal"/> instance to get field values from.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <param name="modifyModal">Delegate that can be used to modify the modal.</param>
        /// <returns>Serialized payload to be used to create a HTTP response.</returns>
        public static string RespondWithModal<T>(this RestInteraction interaction, string customId, T modal, RequestOptions options = null, Action<ModalBuilder> modifyModal = null)
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

            return interaction.RespondWithModal(builder.Build(), options);
        }
    }
}
