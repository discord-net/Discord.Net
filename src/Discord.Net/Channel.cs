using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
    public sealed class Channel
	{
		public sealed class PermissionOverwrite
		{
			public string Type { get; internal set; }
			public string Id { get; internal set; }
			public PackedPermissions Deny { get; internal set; }
			public PackedPermissions Allow { get; internal set; }
		}

		private readonly DiscordClient _client;

		/// <summary> Returns the unique identifier for this channel. </summary>
		public string Id { get; }

		private string _name;
		/// <summary> Returns the name of this channel. </summary>
		public string Name { get { return !IsPrivate ? $"#{_name}" : $"@{Recipient.Name}"; }  internal set { _name = value; } }

		/// <summary> Returns the position of this channel in the channel list for this server. </summary>
		public int Position { get; internal set; }
		/// <summary> Returns false is this is a public chat and true if this is a private chat with another user (see Recipient). </summary>
		public bool IsPrivate { get; }
		/// <summary> Returns the type of this channel (see ChannelTypes). </summary>
		public string Type { get; internal set; }

		/// <summary> Returns the id of the server containing this channel. </summary>
		public string ServerId { get; }
		/// <summary> Returns the server containing this channel. </summary>
		[JsonIgnore]
		public Server Server => ServerId != null ? _client.GetServer(ServerId) : null;

		/// For private chats, returns the Id of the target user, otherwise null.
		[JsonIgnore]
		public string RecipientId { get; internal set; }
		/// For private chats, returns the target user, otherwise null.
		public User Recipient =>  _client.GetUser(RecipientId);

		/// <summary> Returns a collection of all messages the client has in cache. </summary>
		/// <remarks> This collection does not guarantee any ordering. </remarks>
		[JsonIgnore]
		public IEnumerable<Message> Messages => _client.Messages.Where(x => x.ChannelId == Id);

		/// <summary> Returns a collection of all custom permissions used for this channel. </summary>
		public PermissionOverwrite[] PermissionOverwrites { get; internal set; }

		internal Channel(string id, string serverId, DiscordClient client)
		{
			Id = id;
			ServerId = serverId;
			IsPrivate = serverId == null;
			_client = client;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
