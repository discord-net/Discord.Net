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
        public static Task<SocketInteraction> WaitForMessageComponentAsync(BaseSocketClient client, IUserMessage fromMessage, TimeSpan timeout,
            CancellationToken cancellationToken = default)
        {
            bool Predicate(SocketInteraction interaction) => interaction is SocketMessageComponent component &&
                component.Message.Id == fromMessage.Id;

            return WaitForInteractionAsync(client, timeout, Predicate, cancellationToken);
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

            var response = await WaitForMessageComponentAsync(client, prompt, timeout, cancellationToken).ConfigureAwait(false) as SocketMessageComponent;

            await prompt.DeleteAsync().ConfigureAwait(false);

            if (response != null && response.Data.CustomId == confirmId)
                return true;
            else
                return false;
        }
    }
}
