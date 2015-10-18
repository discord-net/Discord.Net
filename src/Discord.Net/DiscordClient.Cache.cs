using System.Collections.Generic;

namespace Discord
{
    public partial class DiscordClient
	{
		/// <summary> Returns the channel with the specified id, or null if none was found. </summary>
		public Channel GetChannel(string id) => _channels[id];
		/// <summary> Returns all channels with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and #Name. Search is case-insensitive. </remarks>
		public IEnumerable<Channel> FindChannels(Server server, string name, string type = null) => _channels.Find(server?.Id, name, type);
		/// <summary> Returns all channels with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and #Name. Search is case-insensitive. </remarks>
		public IEnumerable<Channel> FindChannels(string serverId, string name, string type = null) => _channels.Find(serverId, name, type);

		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public Member GetMember(string serverId, User user) => _members[user?.Id, serverId];
		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public Member GetMember(Server server, User user) => _members[user?.Id, server?.Id];
		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public Member GetMember(Server server, string userId) => _members[userId, server?.Id];
		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public Member GetMember(string serverId, string userId) => _members[userId, serverId];
		/// <summary> Returns all users in with the specified server and name, along with their server-specific data. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive.</remarks>
		public IEnumerable<Member> FindMembers(Server server, string name) => _members.Find(server, name);
		/// <summary> Returns all users in with the specified server and name, along with their server-specific data. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive.</remarks>
		public IEnumerable<Member> FindMembers(string serverId, string name) => _members.Find(_servers[serverId], name);

		/// <summary> Returns the message with the specified id, or null if none was found. </summary>
		public Message GetMessage(string id) => _messages[id];

		/// <summary> Returns the role with the specified id, or null if none was found. </summary>
		public Role GetRole(string id) => _roles[id];
		/// <summary> Returns all roles with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public IEnumerable<Role> FindRoles(Server server, string name) => _roles.Find(server?.Id, name);
		/// <summary> Returns all roles with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public IEnumerable<Role> FindRoles(string serverId, string name) => _roles.Find(serverId, name);

		/// <summary> Returns the server with the specified id, or null if none was found. </summary>
		public Server GetServer(string id) => _servers[id];
		/// <summary> Returns all servers with the specified name. </summary>
		/// <remarks> Search is case-insensitive. </remarks>
		public IEnumerable<Server> FindServers(string name) => _servers.Find(name);

		/// <summary> Returns the user with the specified id, or null if none was found. </summary>
		public User GetUser(string id) => _users[id];
		/// <summary> Returns the user with the specified name and discriminator, or null if none was found. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public User GetUser(string name, string discriminator) => _members[name, discriminator]?.User;
		/// <summary> Returns all users with the specified name across all servers. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public IEnumerable<User> FindUsers(string name) => _users.Find(name);

	}
}