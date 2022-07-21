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
    }
}
