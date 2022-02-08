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
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <returns>A task that represents the asynchronous operation of responding to the interaction.</returns>
        public static async Task RespondWithModalAsync<T>(this IDiscordInteraction interaction, string customId, RequestOptions options = null)
            where T : class, IModal
        {
            if (!ModalUtils.TryGet<T>(out var modalInfo))
                throw new ArgumentException($"{typeof(T).FullName} isn't referenced by any registered Modal Interaction Command and doesn't have a cached {typeof(ModalInfo)}");

            var builder = new ModalBuilder(modalInfo.Title, customId);

            foreach(var input in modalInfo.Components)
                switch (input)
                {
                    case TextInputComponentInfo textComponent:
                        builder.AddTextInput(textComponent.Label, textComponent.CustomId, textComponent.Style, textComponent.Placeholder, textComponent.IsRequired ? textComponent.MinLength : null,
                            textComponent.MaxLength, textComponent.IsRequired, textComponent.InitialValue);
                        break;
                    default:
                        throw new InvalidOperationException($"{input.GetType().FullName} isn't a valid component info class");
                }

            await interaction.RespondWithModalAsync(builder.Build(), options).ConfigureAwait(false);
        }
    }
}
