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

        internal SocketSlashCommandDataOption() { }
        internal SocketSlashCommandDataOption(SocketSlashCommandData data, Model model)
        {
            this.Name = model.Name;
            this.Type = model.Type;

            if (model.Value.IsSpecified)
            {
                switch (Type)
                {
                    case ApplicationCommandOptionType.User:
                    case ApplicationCommandOptionType.Role:
                    case ApplicationCommandOptionType.Channel:
                    case ApplicationCommandOptionType.Mentionable:
                        if (ulong.TryParse($"{model.Value.Value}", out var valueId))
                        {
                            switch (this.Type)
                            {
                                case ApplicationCommandOptionType.User:
                                    {
                                        var guildUser = data.guildMembers.FirstOrDefault(x => x.Key == valueId).Value;

                                        if (guildUser != null)
                                            this.Value = guildUser;
                                        else
                                            this.Value = data.users.FirstOrDefault(x => x.Key == valueId).Value;
                                    }
                                    break;
                                case ApplicationCommandOptionType.Channel:
                                    this.Value = data.channels.FirstOrDefault(x => x.Key == valueId).Value;
                                    break;
                                case ApplicationCommandOptionType.Role:
                                    this.Value = data.roles.FirstOrDefault(x => x.Key == valueId).Value;
                                    break;
                                case ApplicationCommandOptionType.Mentionable:
                                    {
                                        if(data.guildMembers.Any(x => x.Key == valueId) || data.users.Any(x => x.Key == valueId))
                                        {
                                            var guildUser = data.guildMembers.FirstOrDefault(x => x.Key == valueId).Value;

                                            if (guildUser != null)
                                                this.Value = guildUser;
                                            else
                                                this.Value = data.users.FirstOrDefault(x => x.Key == valueId).Value;
                                        }
                                        else if(data.roles.Any(x => x.Key == valueId))
                                        {
                                            this.Value = data.roles.FirstOrDefault(x => x.Key == valueId).Value;
                                        }
                                    }
                                    break;
                                default:
                                    this.Value = model.Value.Value;
                                    break;
                            }
                        }
                        break;
                    case ApplicationCommandOptionType.String:
                        this.Value = model.Value.ToString();
                        break;
                    case ApplicationCommandOptionType.Integer:
                        {
                            if (model.Value.Value is long val)
                                this.Value = val;
                            else if (long.TryParse(model.Value.Value.ToString(), out long res))
                                this.Value = res;
                        }
                        break;
                    case ApplicationCommandOptionType.Boolean:
                        {
                            if (model.Value.Value is bool val)
                                this.Value = val;
                            else if (bool.TryParse(model.Value.Value.ToString(), out bool res))
                                this.Value = res;
                        }
                        break;
                    case ApplicationCommandOptionType.Number:
                        {
                            if (model.Value.Value is int val)
                                this.Value = val;
                            else if (double.TryParse(model.Value.Value.ToString(), out double res))
                                this.Value = res;
                        }
                        break;
                }

            }

            this.Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => new SocketSlashCommandDataOption(data, x)).ToImmutableArray()
                : null;
        }

        // Converters
        public static explicit operator bool(SocketSlashCommandDataOption option)
            => (bool)option.Value;
        public static explicit operator int(SocketSlashCommandDataOption option)
            => (int)option.Value;
        public static explicit operator string(SocketSlashCommandDataOption option)
            => option.Value.ToString();

        IReadOnlyCollection<IApplicationCommandInteractionDataOption> IApplicationCommandInteractionDataOption.Options => this.Options;
    }
}
