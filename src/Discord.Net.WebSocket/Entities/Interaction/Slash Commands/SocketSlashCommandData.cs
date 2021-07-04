using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.ApplicationCommandInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the data tied with the <see cref="SocketSlashCommand"/> interaction.
    /// </summary>
    public class SocketSlashCommandData : SocketEntity<ulong>, IApplicationCommandInteractionData
    {
        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <summary>
        ///     The <see cref="SocketSlashCommandDataOption"/>'s received with this interaction.
        /// </summary>
        public IReadOnlyCollection<SocketSlashCommandDataOption> Options { get; private set; }

        internal Dictionary<ulong, SocketGuildUser> guildMembers { get; private set; }
            = new Dictionary<ulong, SocketGuildUser>();
        internal Dictionary<ulong, SocketGlobalUser> users { get; private set; }
            = new Dictionary<ulong, SocketGlobalUser>();
        internal Dictionary<ulong, SocketChannel> channels { get; private set; }
            = new Dictionary<ulong, SocketChannel>();
        internal Dictionary<ulong, SocketRole> roles { get; private set; }
            = new Dictionary<ulong, SocketRole>();

        private ulong? guildId;

        internal SocketSlashCommandData(DiscordSocketClient client, Model model, ulong? guildId)
            : base(client, model.Id)
        {
            this.guildId = guildId;

            if (model.Resolved.IsSpecified)
            {
                var guild = this.guildId.HasValue ? Discord.GetGuild(this.guildId.Value) : null;

                var resolved = model.Resolved.Value;

                if (resolved.Users.IsSpecified)
                {
                    foreach (var user in resolved.Users.Value)
                    {
                        var socketUser = Discord.GetOrCreateUser(this.Discord.State, user.Value);

                        this.users.Add(ulong.Parse(user.Key), socketUser);
                    }
                }

                if (resolved.Channels.IsSpecified)
                {
                    foreach (var channel in resolved.Channels.Value)
                    {
                        SocketChannel socketChannel = channel.Value.GuildId.IsSpecified
                            ? SocketGuildChannel.Create(Discord.GetGuild(channel.Value.GuildId.Value), Discord.State, channel.Value)
                            : SocketDMChannel.Create(Discord, Discord.State, channel.Value);

                        Discord.State.AddChannel(socketChannel);
                        this.channels.Add(ulong.Parse(channel.Key), socketChannel);
                    }
                }

                if (resolved.Members.IsSpecified)
                {
                    foreach (var member in resolved.Members.Value)
                    {
                        member.Value.User = resolved.Users.Value[member.Key];
                        var user = guild.AddOrUpdateUser(member.Value);
                        this.guildMembers.Add(ulong.Parse(member.Key), user);
                    }
                }

                if (resolved.Roles.IsSpecified)
                {
                    foreach (var role in resolved.Roles.Value)
                    {
                        var socketRole = guild.AddOrUpdateRole(role.Value);
                        this.roles.Add(ulong.Parse(role.Key), socketRole);
                    }
                }
            }
        }

        internal static SocketSlashCommandData Create(DiscordSocketClient client, Model model, ulong id, ulong? guildId)
        {
            var entity = new SocketSlashCommandData(client, model, guildId);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            this.Name = model.Name;

            this.Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => new SocketSlashCommandDataOption(this, x)).ToImmutableArray()
                : null;
        }

        IReadOnlyCollection<IApplicationCommandInteractionDataOption> IApplicationCommandInteractionData.Options => Options;
    }
}
