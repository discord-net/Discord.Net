using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.ApplicationCommandInteractionDataOption;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a Websocket-based <see cref="IApplicationCommandInteractionDataOption"/> recieved by the gateway
    /// </summary>
    public class SocketSlashCommandDataOption : IApplicationCommandInteractionDataOption
    {
        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public object Value { get; private set; }

        /// <summary>
        ///      The sub command options received for this sub command group.
        /// </summary>
        public IReadOnlyCollection<SocketSlashCommandDataOption> Options { get; private set; }

        private DiscordSocketClient discord;

        internal SocketSlashCommandDataOption() { }
        internal SocketSlashCommandDataOption(Model model, DiscordSocketClient discord)
        {
            this.Name = model.Name;
            this.Value = model.Value.IsSpecified ? model.Value.Value : null;
            this.discord = discord;

            this.Options = model.Options.Any()
                ? model.Options.Select(x => new SocketSlashCommandDataOption(x, discord)).ToImmutableArray()
                : null;
        }

        // Converters
        public static explicit operator bool(SocketSlashCommandDataOption option)
            => (bool)option.Value;
        public static explicit operator int(SocketSlashCommandDataOption option)
            => (int)option.Value;
        public static explicit operator string(SocketSlashCommandDataOption option)
            => option.Value.ToString();

        public static explicit operator SocketGuildChannel(SocketSlashCommandDataOption option)
        {
            if (option.Value is ulong id)
            {
                var guild = option.discord.GetGuild(id);

                if (guild == null)
                    return null;

                return guild.GetChannel(id);
            }

            return null;
        }

        public static explicit operator SocketRole(SocketSlashCommandDataOption option)
        {
            if (option.Value is ulong id)
            {
                var guild = option.discord.GetGuild(id);

                if (guild == null)
                    return null;

                return guild.GetRole(id);
            }

            return null;
        }

        public static explicit operator SocketGuildUser(SocketSlashCommandDataOption option)
        {
            if(option.Value is ulong id)
            {
                var guild = option.discord.GetGuild(id);

                if (guild == null)
                    return null;

                return guild.GetUser(id);
            }

            return null;
        }

        IReadOnlyCollection<IApplicationCommandInteractionDataOption> IApplicationCommandInteractionDataOption.Options => this.Options;
    }
}
