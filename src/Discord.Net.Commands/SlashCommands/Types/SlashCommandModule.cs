using Discord.Commands.SlashCommands.Types;
using System;

namespace Discord.SlashCommands
{
    public class SlashCommandModule<T> : ISlashCommandModule  where T : class, IDiscordInteraction
    {
        /// <summary>
        ///     The underlying interaction of the command.
        /// </summary>
        /// <seealso cref="T:Discord.IDiscordInteraction"/>
        /// <seealso cref="T:Discord.WebSocket.SocketInteraction" />
        public T Interaction { get; private set; }

        void ISlashCommandModule.SetContext(IDiscordInteraction interaction)
        {
            var newValue = interaction as T;
            Interaction = newValue ?? throw new InvalidOperationException($"Invalid interaction type. Expected {typeof(T).Name}, got {interaction.GetType().Name}.");
        }
    }
}
