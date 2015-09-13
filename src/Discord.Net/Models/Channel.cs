using Discord.Net.API;
using Newtonsoft.Json;
using System.Collections.Concurrent;
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
		private ConcurrentDictionary<string, bool> _messages;

		/// <summary> Returns the unique identifier for this channel. </summary>
		public string Id { get; }

		private string _name;
		/// <summary> Returns the name of this channel. </summary>
		public string Name { get { return !IsPrivate ? $"{_name}" : $"@{Recipient.Name}"; } internal set { _name = value; } }

		/// <summary> Returns the position of this channel in the channel list for this server. </summary>
		public int Position { get; internal set; }
		/// <summary> Returns false is this is a public chat and true if this is a private chat with another user (see Recipient). </summary>
		public bool IsPrivate => ServerId == null;
		/// <summary> Returns the type of this channel (see ChannelTypes). </summary>
		public string Type { get; internal set; }

		/// <summary> Returns the id of the server containing this channel. </summary>
		public string ServerId { get; }
		/// <summary> Returns the server containing this channel. </summary>
		[JsonIgnore]
		public Server Server => _client.Servers[ServerId];

		/// For private chats, returns the Id of the target user, otherwise null.
		public string RecipientId { get; internal set; }
		/// For private chats, returns the target user, otherwise null.
		[JsonIgnore]
		public User Recipient => _client.Users[RecipientId];

		/// <summary> Returns a collection of the ids of all messages the client has seen posted in this channel. </summary>
		/// <remarks> This collection does not guarantee any ordering. </remarks>
		[JsonIgnore]
		public IEnumerable<string> MessageIds => _messages.Select(x => x.Key);
		/// <summary> Returns a collection of all messages the client has seen posted in this channel. </summary>
		/// <remarks> This collection does not guarantee any ordering. </remarks>
		[JsonIgnore]
		public IEnumerable<Message> Messages => _messages.Select(x => _client.Messages[x.Key]);

		/// <summary> Returns a collection of all custom permissions used for this channel. </summary>
		public PermissionOverwrite[] PermissionOverwrites { get; internal set; }

		internal Channel(DiscordClient client, string id, string serverId, string recipientId)
		{
			_client = client;
			Id = id;
			ServerId = serverId;
			RecipientId = recipientId;
			_messages = new ConcurrentDictionary<string, bool>();
		}

		internal void Update(ChannelReference model)
		{
			Name = model.Name;
			Type = model.Type;
		}
		internal void Update(ChannelInfo model)
		{
			Update(model as ChannelReference);
			
			Position = model.Position;

			if (model.PermissionOverwrites != null)
			{
				PermissionOverwrites = model.PermissionOverwrites.Select(x => new PermissionOverwrite
				{
					Type = x.Type,
					Id = x.Id,
					Deny = new PackedPermissions(true, x.Deny),
					Allow = new PackedPermissions(true, x.Allow)
				}).ToArray();
			}
			else
				PermissionOverwrites = null;
		}

		public override string ToString() => Name;

		internal void AddMessage(string messageId)
		{
			_messages.TryAdd(messageId, true);
		}
		internal bool RemoveMessage(string messageId)
		{
			bool ignored;
			return _messages.TryRemove(messageId, out ignored);
		}
	}
}
