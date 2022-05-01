using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Rest
{
    internal class RestResolvableData<T> where T : API.IResolvable
    {
        internal readonly Dictionary<ulong, RestGuildUser> GuildMembers
           = new Dictionary<ulong, RestGuildUser>();
        internal readonly Dictionary<ulong, RestUser> Users
            = new Dictionary<ulong, RestUser>();
        internal readonly Dictionary<ulong, RestChannel> Channels
            = new Dictionary<ulong, RestChannel>();
        internal readonly Dictionary<ulong, RestRole> Roles
            = new Dictionary<ulong, RestRole>();
        internal readonly Dictionary<ulong, RestMessage> Messages
            = new Dictionary<ulong, RestMessage>();

        internal readonly Dictionary<ulong, Attachment> Attachments
            = new Dictionary<ulong, Attachment>();

        internal async Task PopulateAsync(DiscordRestClient discord, RestGuild guild, IRestMessageChannel channel, T model, bool doApiCall)
        {
            var resolved = model.Resolved.Value;

            if (resolved.Users.IsSpecified)
            {
                foreach (var user in resolved.Users.Value)
                {
                    var restUser = RestUser.Create(discord, user.Value);

                    Users.Add(ulong.Parse(user.Key), restUser);
                }
            }

            if (resolved.Channels.IsSpecified)
            {
                var channels = doApiCall ? await guild.GetChannelsAsync().ConfigureAwait(false) : null;

                foreach (var channelModel in resolved.Channels.Value)
                {
                    if (channels != null)
                    {
                        var guildChannel = channels.FirstOrDefault(x => x.Id == channelModel.Value.Id);

                        guildChannel.Update(channelModel.Value);

                        Channels.Add(ulong.Parse(channelModel.Key), guildChannel);
                    }
                    else
                    {
                        var restChannel = RestChannel.Create(discord, channelModel.Value);

                        restChannel.Update(channelModel.Value);

                        Channels.Add(ulong.Parse(channelModel.Key), restChannel);
                    }
                }
            }

            if (resolved.Members.IsSpecified)
            {
                foreach (var member in resolved.Members.Value)
                {
                    // pull the adjacent user model
                    member.Value.User = resolved.Users.Value.FirstOrDefault(x => x.Key == member.Key).Value;
                    var restMember = RestGuildUser.Create(discord, guild, member.Value);

                    GuildMembers.Add(ulong.Parse(member.Key), restMember);
                }
            }

            if (resolved.Roles.IsSpecified)
            {
                foreach (var role in resolved.Roles.Value)
                {
                    var restRole = RestRole.Create(discord, guild, role.Value); 

                    Roles.Add(ulong.Parse(role.Key), restRole);
                }
            }

            if (resolved.Messages.IsSpecified)
            {
                foreach (var msg in resolved.Messages.Value)
                {
                    channel ??= (IRestMessageChannel)(Channels.FirstOrDefault(x => x.Key == msg.Value.ChannelId).Value
                        ?? (doApiCall
                        ? await discord.GetChannelAsync(msg.Value.ChannelId).ConfigureAwait(false)
                        : null));

                    RestUser author;

                    if (msg.Value.Author.IsSpecified)
                    {
                        author = RestUser.Create(discord, msg.Value.Author.Value);
                    }
                    else
                    {
                        author = RestGuildUser.Create(discord, guild, msg.Value.Member.Value);
                    }

                    var message = RestMessage.Create(discord, channel, author, msg.Value);

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
