using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public sealed class Channel
	{
		private readonly DiscordClient _client;

		public string Id { get; }

		private string _name;
		public string Name { get { return !IsPrivate ? _name : '@' + Recipient.Name; }  internal set { _name = value; } }

		public bool IsPrivate { get;  }
		public string Type { get; internal set; }
		
		public string ServerId { get; }
		[JsonIgnore]
		public Server Server { get { return ServerId != null ? _client.GetServer(ServerId) : null; } }

		[JsonIgnore]
		public string RecipientId { get; internal set; }
		public User Recipient { get { return _client.GetUser(RecipientId); } }

		public IEnumerable<Message> Messages { get { return _client.Messages.Where(x => x.ChannelId == Id); } }

		//Not Implemented
		public object[] PermissionOverwrites { get; internal set; }

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
