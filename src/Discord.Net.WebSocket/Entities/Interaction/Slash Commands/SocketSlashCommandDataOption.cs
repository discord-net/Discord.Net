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

        /// <inheritdoc/>
        public ApplicationCommandOptionType Type { get; private set; }

        /// <summary>
        ///      The sub command options received for this sub command group.
        /// </summary>
        public IReadOnlyCollection<SocketSlashCommandDataOption> Options { get; private set; }


        private SocketSlashCommandData data;

        internal SocketSlashCommandDataOption() { }
        internal SocketSlashCommandDataOption(SocketSlashCommandData data, Model model)
        {
            this.Name = model.Name;
            this.Value = model.Value.IsSpecified ? model.Value.Value : null;

            this.Options = model.Options.Any()
                ? model.Options.Select(x => new SocketSlashCommandDataOption(data, x)).ToImmutableArray()
                : null;
        }

        // Converters
        public static explicit operator bool(SocketSlashCommandDataOption option)
            => (bool)option.Value;
        public static explicit operator int(SocketSlashCommandDataOption option)
            => (int)option.Value;
        public static explicit operator string(SocketSlashCommandDataOption option)
            => option.Value.ToString();

        public static explicit operator SocketChannel(SocketSlashCommandDataOption option)
        {
            if(ulong.TryParse(option.Value.ToString(), out ulong id))
            {
                if (option.data.channels.TryGetValue(id, out var channel))
                    return channel;
            }

            return null;
        }

        public static explicit operator SocketRole(SocketSlashCommandDataOption option)
        {
            if (ulong.TryParse(option.Value.ToString(), out ulong id))
            {
                if (option.data.roles.TryGetValue(id, out var role))
                    return role;
            }

            return null;
        }

        public static explicit operator SocketUser(SocketSlashCommandDataOption option)
        {
            if (ulong.TryParse(option.Value.ToString(), out ulong id))
            {
                if (option.data.users.TryGetValue(id, out var user))
                    return user;
            }

            return null;
        }

        public static explicit operator SocketGuildUser(SocketSlashCommandDataOption option)
        {
            if (option.Value as SocketUser is SocketGuildUser guildUser)
                return guildUser;

            return null;
        }


        IReadOnlyCollection<IApplicationCommandInteractionDataOption> IApplicationCommandInteractionDataOption.Options => this.Options;
    }
}
