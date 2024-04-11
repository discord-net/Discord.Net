using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Utility class containing helper methods for interacting with Discord Interactions.
    /// </summary>
    public static class InteractionUtility
    {
        /// <summary>
        ///     Wait for an Interaction event for a given amount of time as an asynchronous operation.
        /// </summary>
        /// <param name="client">Client that should be listened to for the <see cref="BaseSocketClient.InteractionCreated"/> event.</param>
        /// <param name="timeout">Timeout duration for this operation.</param>
        /// <param name="predicate">Delegate for checking whether an Interaction meets the requirements.</param>
        /// <param name="cancellationToken">Token for canceling the wait operation.</param>
        /// <returns>
        ///     A Task representing the asynchronous waiting operation. If the user responded in the given amount of time, Task result contains the user response,
        ///     otherwise the Task result is <see langword="null"/>.
        /// </returns>
        public static async Task<SocketInteraction> WaitForInteractionAsync(BaseSocketClient client, TimeSpan timeout,
            Predicate<SocketInteraction> predicate, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<SocketInteraction>();

            var waitCancelSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            Task wait = Task.Delay(timeout, waitCancelSource.Token)
                .ContinueWith((t) =>
                {
                    if (!t.IsCanceled)
                        tcs.SetResult(null);
                });

            cancellationToken.Register(() => tcs.SetCanceled());

            client.InteractionCreated += HandleInteraction;
            var result = await tcs.Task.ConfigureAwait(false);
            client.InteractionCreated -= HandleInteraction;

            return result;

            Task HandleInteraction(SocketInteraction interaction)
            {
                if (predicate(interaction))
                {
                    waitCancelSource.Cancel();
                    tcs.SetResult(interaction);
                }

                return Task.CompletedTask;
            }
        }

        /// <summary>
        ///    Wait for an Message Component Interaction event for a given amount of time as an asynchronous operation .
        /// </summary>
        /// <param name="client">Client that should be listened to for the <see cref="BaseSocketClient.InteractionCreated"/> event.</param>
        /// <param name="fromMessage">The message that <see cref="BaseSocketClient.ButtonExecuted"/> or <see cref="BaseSocketClient.SelectMenuExecuted"/> should originate from.</param>
        /// <param name="timeout">Timeout duration for this operation.</param>
        /// <param name="cancellationToken">Token for canceling the wait operation.</param>
        /// <returns>
        ///     A Task representing the asynchronous waiting operation with a <see cref="IDiscordInteraction"/> result,
        ///     the result is null if the process timed out before receiving a valid Interaction.
        /// </returns>
        public static async Task<SocketMessageComponent> WaitForMessageComponentAsync(BaseSocketClient client, IUserMessage fromMessage, TimeSpan timeout,
            CancellationToken cancellationToken = default)
        {
            bool Predicate(SocketInteraction interaction) => interaction is SocketMessageComponent component &&
                component.Message.Id == fromMessage.Id;

            return await WaitForInteractionAsync(client, timeout, Predicate, cancellationToken) as SocketMessageComponent;
        }

        /// <summary>
        ///     Create a confirmation dialog and wait for user input asynchronously.
        /// </summary>
        /// <param name="client">Client that should be listened to for the <see cref="BaseSocketClient.InteractionCreated"/> event.</param>
        /// <param name="channel">Send the confirmation prompt to this channel.</param>
        /// <param name="timeout">Timeout duration of this operation.</param>
        /// <param name="message">Optional custom prompt message.</param>
        /// <param name="cancellationToken">Token for canceling the wait operation.</param>
        /// <returns>
        ///     A Task representing the asynchronous waiting operation with a <see cref="bool"/> result,
        ///     the result is <see langword="false"/> if the user declined the prompt or didnt answer in time, <see langword="true"/> if the user confirmed the prompt.
        /// </returns>
        public static async Task<bool> ConfirmAsync(BaseSocketClient client, IMessageChannel channel, TimeSpan timeout, string message = null,
            CancellationToken cancellationToken = default)
        {
            message ??= "Would you like to continue?";
            var confirmId = $"confirm";
            var declineId = $"decline";

            var component = new ComponentBuilder()
                .WithButton("Confirm", confirmId, ButtonStyle.Success)
                .WithButton("Cancel", declineId, ButtonStyle.Danger)
                .Build();

            var prompt = await channel.SendMessageAsync(message, components: component).ConfigureAwait(false);

            var response = await WaitForMessageComponentAsync(client, prompt, timeout, cancellationToken).ConfigureAwait(false);
            await prompt.DeleteAsync().ConfigureAwait(false);

            return response is not null && response.Data.CustomId == confirmId;
        }

        /// <summary>
        ///     Create a confirmation dialog and wait for user input asynchronously.
        /// </summary>
        /// <param name="interaction">Interaction to send the response/followup message to.</param>
        /// <param name="timeout">Timeout duration of this operation.</param>
        /// <param name="message">Optional custom prompt message.</param>
        /// <param name="cancellationToken">Token for canceling the wait operation.</param>
        /// <returns>
        ///     A Task representing the asyncronous waiting operation with a <see cref="bool"/> result,
        ///     the result is <see langword="false"/> if the user declined the prompt or didnt answer in time, <see langword="true"/> if the user confirmed the prompt.
        /// </returns>
        public static async Task<bool> ConfirmAsync(SocketInteraction interaction, TimeSpan timeout, string message = null, Action<MessageProperties> updateMessage = null,
            CancellationToken cancellationToken = default)
        {
            message ??= "Would you like to continue?";
            var confirmId = $"confirm";
            var declineId = $"decline";

            var component = new ComponentBuilder()
                .WithButton("Confirm", confirmId, ButtonStyle.Success)
                .WithButton("Cancel", declineId, ButtonStyle.Danger)
                .Build();

            IUserMessage prompt;

            if (!interaction.HasResponded)
            {
                await interaction.RespondAsync(message, components: component, ephemeral: true);
                prompt = await interaction.GetOriginalResponseAsync();
            }
            else
                prompt = await interaction.FollowupAsync(message, components: component, ephemeral: true);

            var response = await WaitForMessageComponentAsync(interaction.Discord, prompt, timeout, cancellationToken).ConfigureAwait(false);

            if(updateMessage is not null)
                await response.UpdateAsync(updateMessage);

            return response is not null && response.Data.CustomId == confirmId;
        }

        /// <summary>
        ///     Responds to an interaction with a modal and asyncronously wait for the users response.
        /// </summary>
        /// <typeparam name="TModal">The type of <see cref="IModal"/> to respond with.</typeparam>
        /// <param name="interaction">The interaction to respond to.</param>
        /// <param name="timeout">Timeout duration of this operation.</param>
        /// <param name="contextFactory">Delegate for creating <see cref="IInteractionContext"/>s to be passed on to the <see cref="ComponentTypeConverter"/>s.</param>
        /// <param name="services">Service collection to be passed on to the <see cref="ComponentTypeConverter"/>s.</param>
        /// <param name="cancellationToken">Token for canceling the wait operation.</param>
        /// <returns>
        ///     A Task representing the asyncronous waiting operation with a <typeparamref name="TModal"/> result,
        ///     the result is <see langword="null"/>q if the process timed out before receiving a valid Interaction.
        /// </returns>
        public static async Task<TModal> SendModalAsync<TModal>(this SocketInteraction interaction, TimeSpan timeout,
            Func<SocketModal, DiscordSocketClient, IInteractionContext> contextFactory, IServiceProvider services = null, CancellationToken cancellationToken = default)
            where TModal : class, IModal
        {
            var customId = Guid.NewGuid().ToString();
            await interaction.RespondWithModalAsync<TModal>(customId);
            var response = await WaitForInteractionAsync(interaction.Discord, timeout, interaction =>
            {
                return interaction is SocketModal socketModal &&
                socketModal.Data.CustomId == customId;
            }, cancellationToken) as SocketModal;

            var modal = await ModalUtils.CreateModalAsync<TModal>(contextFactory(response, response.Discord), services).ConfigureAwait(false);
            return modal;
        }
    }
}
