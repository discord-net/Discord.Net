using System.Collections.Generic;

namespace Discord.WebSocket
{
    internal class SocketResolvableData<T> where T : API.IResolvable
    {
        internal readonly Dictionary<ulong, SocketGuildUser> GuildMembers
           = new Dictionary<ulong, SocketGuildUser>();
        internal readonly Dictionary<ulong, SocketGlobalUser> Users
            = new Dictionary<ulong, SocketGlobalUser>();
        internal readonly Dictionary<ulong, SocketChannel> Channels
            = new Dictionary<ulong, SocketChannel>();
        internal readonly Dictionary<ulong, SocketRole> Roles
            = new Dictionary<ulong, SocketRole>();

        internal readonly Dictionary<ulong, SocketMessage> Messages
            = new Dictionary<ulong, SocketMessage>();

        internal readonly Dictionary<ulong, Attachment> Attachments
            = new Dictionary<ulong, Attachment>();

        internal SocketResolvableData(DiscordSocketClient discord, ulong? guildId, T model)
        {
            var guild = guildId.HasValue ? discord.GetGuild(guildId.Value) : null;

            var resolved = model.Resolved.Value;

            if (resolved.Users.IsSpecified)
            {
                foreach (var user in resolved.Users.Value)
                {
                    var socketUser = discord.GetOrCreateUser(discord.State, user.Value);

                    Users.Add(ulong.Parse(user.Key), socketUser);
                }
            }

            if (resolved.Channels.IsSpecified)
            {
                foreach (var channel in resolved.Channels.Value)
                {
                    SocketChannel socketChannel = guild != null
                        ? guild.GetChannel(channel.Value.Id)
                        : discord.GetChannel(channel.Value.Id);

                    if (socketChannel == null)
                    {
                        var channelModel = guild != null
                            ? discord.Rest.ApiClient.GetChannelAsync(guild.Id, channel.Value.Id).ConfigureAwait(false).GetAwaiter().GetResult()
                            : discord.Rest.ApiClient.GetChannelAsync(channel.Value.Id).ConfigureAwait(false).GetAwaiter().GetResult();

                        socketChannel = guild != null
                            ? SocketGuildChannel.Create(guild, discord.State, channelModel)
                            : (SocketChannel)SocketChannel.CreatePrivate(discord, discord.State, channelModel);
                    }

                    discord.State.AddChannel(socketChannel);
                    Channels.Add(ulong.Parse(channel.Key), socketChannel);
                }
            }

            if (resolved.Members.IsSpecified && guild != null)
            {
                foreach (var member in resolved.Members.Value)
                {
                    member.Value.User = resolved.Users.Value[member.Key];
                    var user = guild.AddOrUpdateUser(member.Value);
                    GuildMembers.Add(ulong.Parse(member.Key), user);
                }
            }

            if (resolved.Roles.IsSpecified)
            {
                foreach (var role in resolved.Roles.Value)
                {
                    var socketRole = guild.AddOrUpdateRole(role.Value);
                    Roles.Add(ulong.Parse(role.Key), socketRole);
                }
            }

            if (resolved.Messages.IsSpecified)
            {
                foreach (var msg in resolved.Messages.Value)
                {
                    var channel = discord.GetChannel(msg.Value.ChannelId) as ISocketMessageChannel;

                    SocketUser author;
                    if (guild != null)
                    {
                        if (msg.Value.WebhookId.IsSpecified)
                            author = SocketWebhookUser.Create(guild, discord.State, msg.Value.Author.Value, msg.Value.WebhookId.Value);
                        else
                            author = guild.GetUser(msg.Value.Author.Value.Id);
                    }
                    else
                        author = (channel as SocketChannel).GetUser(msg.Value.Author.Value.Id);

                    if (channel == null)
                    {
                        if (!msg.Value.GuildId.IsSpecified)  // assume it is a DM
                        {
                            channel = discord.CreateDMChannel(msg.Value.ChannelId, msg.Value.Author.Value, discord.State);
                        }
                    }

                    var message = SocketMessage.Create(discord, discord.State, author, channel, msg.Value);
                    Messages.Add(message.Id, message);
                }
            }

            if (resolved.Attachments.IsSpecified)
            {
                foreach (var attachment in resolved.Attachments.Value)
                {
                    var discordAttachment = Attachment.Create(attachment.Value);

                    Attachments.Add(ulong.Parse(attachment.Key), discordAttachment);
                }
            }
        }
    }
}
