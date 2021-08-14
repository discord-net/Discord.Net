using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.ApplicationCommandInteractionData;

namespace Discord.WebSocket
{
    public class SocketCommandBaseData : SocketEntity<ulong>, IApplicationCommandInteractionData
    {
        public string Name { get; private set; }

        public IReadOnlyCollection<SocketCommandBaseDataOption> Options { get; private set; }
        // id
        // type
        internal Dictionary<ulong, SocketGuildUser> guildMembers { get; private set; }
            = new Dictionary<ulong, SocketGuildUser>();
        internal Dictionary<ulong, SocketGlobalUser> users { get; private set; }
            = new Dictionary<ulong, SocketGlobalUser>();
        internal Dictionary<ulong, SocketChannel> channels { get; private set; }
            = new Dictionary<ulong, SocketChannel>();
        internal Dictionary<ulong, SocketRole> roles { get; private set; }
            = new Dictionary<ulong, SocketRole>();

        private ulong? guildId;

        internal SocketMessage Message { get; private set; }

        private ApplicationCommandType Type { get; set; }

        internal SocketCommandBaseData(DiscordSocketClient client, Model model, ulong? guildId)
            : base(client, model.Id)
        {
            this.guildId = guildId;

            this.Type = (ApplicationCommandType)model.Type;

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
                        SocketChannel socketChannel = guild != null
                            ? guild.GetChannel(channel.Value.Id)
                            : Discord.GetChannel(channel.Value.Id);

                        if (socketChannel == null)
                        {
                            var channelModel = guild != null
                                ? Discord.Rest.ApiClient.GetChannelAsync(guild.Id, channel.Value.Id).ConfigureAwait(false).GetAwaiter().GetResult()
                                : Discord.Rest.ApiClient.GetChannelAsync(channel.Value.Id).ConfigureAwait(false).GetAwaiter().GetResult();

                            socketChannel = guild != null
                                ? SocketGuildChannel.Create(guild, Discord.State, channelModel)
                                : (SocketChannel)SocketChannel.CreatePrivate(Discord, Discord.State, channelModel);
                        }

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

                if (resolved.Messages.IsSpecified)
                {
                    foreach (var msg in resolved.Messages.Value)
                    {
                        var channel = client.GetChannel(msg.Value.ChannelId) as ISocketMessageChannel;

                        SocketUser author;
                        if (guild != null)
                        {
                            if (msg.Value.WebhookId.IsSpecified)
                                author = SocketWebhookUser.Create(guild, client.State, msg.Value.Author.Value, msg.Value.WebhookId.Value);
                            else
                                author = guild.GetUser(msg.Value.Author.Value.Id);
                        }
                        else
                            author = (channel as SocketChannel).GetUser(msg.Value.Author.Value.Id);

                        if (channel == null)
                        {
                            if (!msg.Value.GuildId.IsSpecified)  // assume it is a DM
                            {
                                channel = client.CreateDMChannel(msg.Value.ChannelId, msg.Value.Author.Value, client.State);
                            }
                        }

                        this.Message = SocketMessage.Create(client, client.State, author, channel, msg.Value);
                    }
                }
            }
        }

        internal static SocketCommandBaseData Create(DiscordSocketClient client, Model model, ulong id, ulong? guildId)
        {
            var entity = new SocketCommandBaseData(client, model, guildId);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            this.Name = model.Name;

            this.Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => new SocketCommandBaseDataOption(this, x)).ToImmutableArray()
                : null;
        }

        IReadOnlyCollection<IApplicationCommandInteractionDataOption> IApplicationCommandInteractionData.Options => Options;
    }
}
