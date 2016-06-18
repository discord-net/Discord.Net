using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord.Legacy
{
    public static class Mention
    {
        /// <summary> Returns the string used to create a user mention. </summary>
        [Obsolete("Use User.Mention instead")]
        public static string User(User user)
            => user.Mention;
        /// <summary> Returns the string used to create a channel mention. </summary>
        [Obsolete("Use Channel.Mention instead")]
        public static string Channel(Channel channel)
            => channel.Mention;
        /// <summary> Returns the string used to create a mention to everyone in a channel. </summary>
        [Obsolete("Use Server.EveryoneRole.Mention instead")]
        public static string Everyone()
            => $"@everyone";
    }

    public static class LegacyExtensions
    {
        [Obsolete("Use DiscordClient.ExecuteAndWait")]
        public static void Run(this DiscordClient client, Func<Task> asyncAction)
        {
            client.ExecuteAndWait(asyncAction);
        }

        [Obsolete("Use Server.FindChannels")]
        public static IEnumerable<Channel> FindChannels(this DiscordClient client, Server server, string name, ChannelType type = null, bool exactMatch = false)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.FindChannels(name, type, exactMatch);
        }

        [Obsolete("Use Server.CreateChannel")]
        public static Task<Channel> CreateChannel(this DiscordClient client, Server server, string name, ChannelType type)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.CreateChannel(name, type);
        }
        [Obsolete("Use User.CreateChannel")]
        public static Task<Channel> CreatePMChannel(this DiscordClient client, User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return user.CreatePMChannel();
        }
        [Obsolete("Use Channel.Edit")]
        public static Task EditChannel(this DiscordClient client, Channel channel, string name = null, string topic = null, int? position = null)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.Edit(name, topic, position);
        }
        [Obsolete("Use Channel.Delete")]
        public static Task DeleteChannel(this DiscordClient client, Channel channel)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.Delete();
        }

        [Obsolete("Use Server.ReorderChannels")]
        public static Task ReorderChannels(this DiscordClient client, Server server, IEnumerable<Channel> channels, Channel after = null)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.ReorderChannels(channels, after);
        }

        [Obsolete("Use Server.GetInvites")]
        public static Task<IEnumerable<Invite>> GetInvites(this DiscordClient client, Server server)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.GetInvites();
        }

        [Obsolete("Use Server.CreateInvite")]
        public static Task<Invite> CreateInvite(this DiscordClient client, Server server, int? maxAge = 1800, int? maxUses = null, bool tempMembership = false, bool withXkcd = false)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.CreateInvite(maxAge, maxUses, tempMembership, withXkcd);
        }
        [Obsolete("Use Channel.CreateInvite")]
        public static Task<Invite> CreateInvite(this DiscordClient client, Channel channel, int? maxAge = 1800, int? maxUses = null, bool tempMembership = false, bool withXkcd = false)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.CreateInvite(maxAge, maxUses, tempMembership, withXkcd);
        }

        [Obsolete("Use Invite.Delete")]
        public static Task DeleteInvite(this DiscordClient client, Invite invite)
        {
            if (invite == null) throw new ArgumentNullException(nameof(invite));
            return invite.Delete();
        }
        [Obsolete("Use Invite.Accept")]
        public static Task AcceptInvite(this DiscordClient client, Invite invite)
        {
            if (invite == null) throw new ArgumentNullException(nameof(invite));
            return invite.Accept();
        }

        [Obsolete("Use Channel.SendMessage")]
        public static Task<Message> SendMessage(this DiscordClient client, Channel channel, string text)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.SendMessage(text);
        }
        [Obsolete("Use Channel.SendTTSMessage")]
        public static Task<Message> SendTTSMessage(this DiscordClient client, Channel channel, string text)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.SendTTSMessage(text);
        }
        [Obsolete("Use Channel.SendFile")]
        public static Task<Message> SendFile(this DiscordClient client, Channel channel, string filePath)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.SendFile(filePath);
        }
        [Obsolete("Use Channel.SendFile")]
        public static Task<Message> SendFile(this DiscordClient client, Channel channel, string filename, Stream stream)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.SendFile(filename, stream);
        }
        [Obsolete("Use User.SendMessage")]
        public static Task<Message> SendMessage(this DiscordClient client, User user, string text)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return user.SendMessage(text);
        }
        [Obsolete("Use User.SendFile")]
        public static Task<Message> SendFile(this DiscordClient client, User user, string filePath)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return user.SendFile(filePath);
        }
        [Obsolete("Use User.SendFile")]
        public static Task<Message> SendFile(this DiscordClient client, User user, string filename, Stream stream)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return user.SendFile(filename, stream);
        }

        [Obsolete("Use Message.Edit")]
        public static Task EditMessage(this DiscordClient client, Message message, string text)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            return message.Edit(text);
        }

        [Obsolete("Use Message.Delete")]
        public static Task DeleteMessage(this DiscordClient client, Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            return message.Delete();
        }
        [Obsolete("Use Message.Delete")]
        public static async Task DeleteMessages(this DiscordClient client, IEnumerable<Message> messages)
        {
            if (messages == null) throw new ArgumentNullException(nameof(messages));

            foreach (var message in messages)
                await message.Delete().ConfigureAwait(false);
        }

        [Obsolete("Use Channel.DownloadMessages")]
        public static Task<Message[]> DownloadMessages(this DiscordClient client, Channel channel, int limit = 100, ulong? relativeMessageId = null, Relative relativeDir = Relative.Before, bool useCache = true)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.DownloadMessages(limit, relativeMessageId, relativeDir, useCache);
        }

        [Obsolete("Use Server.GetUser")]
        public static User GetUser(this DiscordClient client, Server server, ulong userId)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.GetUser(userId);
        }
        [Obsolete("Use Server.GetUser")]
        public static User GetUser(this DiscordClient client, Server server, string username, ushort discriminator)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.GetUser(username, discriminator);
        }

        [Obsolete("Use Server.FindUsers")]
        public static IEnumerable<User> FindUsers(this DiscordClient client, Server server, string name, bool exactMatch = false)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.FindUsers(name, exactMatch);
        }
        [Obsolete("Use Channel.FindUsers")]
        public static IEnumerable<User> FindUsers(this DiscordClient client, Channel channel, string name, bool exactMatch = false)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.FindUsers(name, exactMatch);
        }

        [Obsolete("Use User.Edit")]
        public static Task EditUser(this DiscordClient client, User user, bool? isMuted = null, bool? isDeafened = null, Channel voiceChannel = null, IEnumerable<Role> roles = null)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return user.Edit(isMuted, isDeafened, voiceChannel, roles);
        }

        [Obsolete("Use User.Kick")]
        public static Task KickUser(this DiscordClient client, User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return user.Kick();
        }
        [Obsolete("Use Server.Ban")]
        public static Task BanUser(this DiscordClient client, User user, int pruneDays = 0)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var server = user.Server;
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.Ban(user, pruneDays);
        }
        [Obsolete("Use Server.Unban")]
        public static Task UnbanUser(this DiscordClient client, User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var server = user.Server;
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.Unban(user);
        }
        [Obsolete("Use Server.Unban")]
        public static Task UnbanUser(this DiscordClient client, Server server, ulong userId)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.Unban(userId);
        }

        [Obsolete("Use Server.PruneUsers")]
        public static Task<int> PruneUsers(this DiscordClient client, Server server, int days = 30, bool simulate = false)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.PruneUsers(days, simulate);
        }

        [Obsolete("Use DiscordClient.CurrentUser.Edit")]
        public static Task EditProfile(this DiscordClient client, string currentPassword = "",
            string username = null, string email = null, string password = null,
            Stream avatar = null, ImageType avatarType = ImageType.Png)
            => client.CurrentUser.Edit(currentPassword, username, email, password, avatar, avatarType);

        [Obsolete("Use Server.GetRole")]
        public static Role GetRole(this DiscordClient client, Server server, ulong id)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.GetRole(id);
        }
        [Obsolete("Use Server.FindRoles")]
        public static IEnumerable<Role> FindRoles(this DiscordClient client, Server server, string name)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.FindRoles(name);
        }

        [Obsolete("Use Server.CreateRole")]
        public static Task<Role> CreateRole(this DiscordClient client, Server server, string name, ServerPermissions? permissions = null, Color color = null, bool isHoisted = false)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.CreateRole(name, permissions, color);
        }
        [Obsolete("Use Role.Edit")]
        public static Task EditRole(this DiscordClient client, Role role, string name = null, ServerPermissions? permissions = null, Color color = null, bool? isHoisted = null, int? position = null)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            return role.Edit(name, permissions, color, isHoisted, position);
        }

        [Obsolete("Use Role.Delete")]
        public static Task DeleteRole(this DiscordClient client, Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            return role.Delete();
        }

        [Obsolete("Use Server.ReorderRoles")]
        public static Task ReorderRoles(this DiscordClient client, Server server, IEnumerable<Role> roles, Role after = null)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.ReorderRoles(roles, after);
        }

        [Obsolete("Use Server.Edit")]
        public static Task EditServer(this DiscordClient client, Server server, string name = null, string region = null, Stream icon = null, ImageType iconType = ImageType.Png)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.Edit(name, region, icon, iconType);
        }

        [Obsolete("Use Server.Leave")]
        public static Task LeaveServer(this DiscordClient client, Server server)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return server.Leave();
        }

        [Obsolete("Use DiscordClient.Regions")]
        public static IEnumerable<Region> GetVoiceRegions(this DiscordClient client)
            => client.Regions;

        [Obsolete("Use Channel.GetPermissionRule")]
        public static ChannelPermissionOverrides GetChannelPermissions(this DiscordClient client, Channel channel, User user)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.GetPermissionsRule(user);
        }
        [Obsolete("Use Channel.GetPermissionRule")]
        public static ChannelPermissionOverrides GetChannelPermissions(this DiscordClient client, Channel channel, Role role)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.GetPermissionsRule(role);
        }
        [Obsolete("Use Channel.AddPermissionRule(DualChannelPermissions)", true)]
        public static Task SetChannelPermissions(this DiscordClient client, Channel channel, User user, ChannelPermissions allow, ChannelPermissions deny)
        {
            throw new InvalidOperationException();
        }
        [Obsolete("Use Channel.AddPermissionRule")]
        public static Task SetChannelPermissions(this DiscordClient client, Channel channel, User user, ChannelPermissionOverrides permissions)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.AddPermissionsRule(user, permissions);
        }
        [Obsolete("Use Channel.AddPermissionRule(DualChannelPermissions)")]
        public static Task SetChannelPermissions(this DiscordClient client, Channel channel, Role role, ChannelPermissions allow, ChannelPermissions deny)
        {
            throw new InvalidOperationException();
        }
        [Obsolete("Use Channel.AddPermissionRule")]
        public static Task SetChannelPermissions(this DiscordClient client, Channel channel, Role role, ChannelPermissionOverrides permissions)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.AddPermissionsRule(role, permissions);
        }
        [Obsolete("Use Channel.RemovePermissionRule")]
        public static Task RemoveChannelPermissions(this DiscordClient client, Channel channel, User user)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.RemovePermissionsRule(user);
        }
        [Obsolete("Use Channel.RemovePermissionRule")]
        public static Task RemoveChannelPermissions(this DiscordClient client, Channel channel, Role role)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.RemovePermissionsRule(role);
        }

        [Obsolete("Removed", true)]
        public static Task AckMessage(this DiscordClient client, Message message)
        {
            throw new InvalidOperationException();
        }
        [Obsolete("Use Channel.ImportMessages", true)]
        public static IEnumerable<Message> ImportMessages(Channel channel, string json)
        {
            throw new InvalidOperationException();
        }
        [Obsolete("Use Channel.ExportMessages", true)]
        public static string ExportMessages(Channel channel)
        {
            throw new InvalidOperationException();
        }
    }
}
