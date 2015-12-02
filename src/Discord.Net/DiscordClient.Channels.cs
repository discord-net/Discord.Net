using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed class Channels : AsyncCollection<long, Channel>
	{
		public IEnumerable<Channel> PrivateChannels => _privateChannels.Select(x => x.Value);
		private ConcurrentDictionary<long, Channel> _privateChannels;

		public Channels(DiscordClient client, object writerLock)
			: base(client, writerLock)
		{
			_privateChannels = new ConcurrentDictionary<long, Channel>();
			ItemCreated += (s, e) =>
			{
				if (e.Item.IsPrivate)
					_privateChannels.TryAdd(e.Item.Id, e.Item);
			};
			ItemDestroyed += (s, e) =>
			{
				if (e.Item.IsPrivate)
				{
					Channel ignored;
					_privateChannels.TryRemove(e.Item.Id, out ignored);
				}
			};
			Cleared += (s, e) => _privateChannels.Clear();
        }
		
		public Channel GetOrAdd(long id, long? serverId, long? recipientId = null)
			=> GetOrAdd(id, () => new Channel(_client, id, serverId, recipientId));
	}

	public class ChannelEventArgs : EventArgs
	{
		public Channel Channel { get; }
		public Server Server => Channel.Server;

		public ChannelEventArgs(Channel channel) { Channel = channel; }
	}

	public partial class DiscordClient
	{
		public event AsyncEventHandler<ChannelEventArgs> ChannelCreated { add { _channelCreated.Add(value); } remove { _channelCreated.Remove(value); } }
		private readonly AsyncEvent<ChannelEventArgs> _channelCreated = new AsyncEvent<ChannelEventArgs>(nameof(ChannelCreated));
		private Task RaiseChannelCreated(Channel channel)
			=> RaiseEvent(_channelCreated, new ChannelEventArgs(channel));

		public event AsyncEventHandler<ChannelEventArgs> ChannelDestroyed { add { _channelDestroyed.Add(value); } remove { _channelDestroyed.Remove(value); } }
		private readonly AsyncEvent<ChannelEventArgs> _channelDestroyed = new AsyncEvent<ChannelEventArgs>(nameof(ChannelDestroyed));
		private Task RaiseChannelDestroyed(Channel channel)
			=> RaiseEvent(_channelDestroyed, new ChannelEventArgs(channel));

		public event AsyncEventHandler<ChannelEventArgs> ChannelUpdated { add { _channelUpdated.Add(value); } remove { _channelUpdated.Remove(value); } }
		private readonly AsyncEvent<ChannelEventArgs> _channelUpdated = new AsyncEvent<ChannelEventArgs>(nameof(ChannelUpdated));
		private Task RaiseChannelUpdated(Channel channel)
			=> RaiseEvent(_channelUpdated, new ChannelEventArgs(channel));

		/// <summary> Returns a collection of all servers this client is a member of. </summary>
		public IEnumerable<Channel> PrivateChannels { get { CheckReady(); return _channels.PrivateChannels; } }
		internal Channels Channels => _channels;
		private readonly Channels _channels;

		/// <summary> Returns the channel with the specified id, or null if none was found. </summary>
		public Channel GetChannel(long id)
		{
			if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
			CheckReady();

			return _channels[id];
		}
		
		/// <summary> Returns all channels with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and #Name. Search is case-insensitive. </remarks>
		public IEnumerable<Channel> FindChannels(Server server, string name, ChannelType type = null, bool exactMatch = false)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (name == null) throw new ArgumentNullException(nameof(name));
			CheckReady();
			
			var query = server.Channels.Where(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));

			if (!exactMatch && name.Length >= 2)
			{
				if (name[0] == '<' && name[1] == '#' && name[name.Length - 1] == '>') //Parse mention
				{					
					long id = IdConvert.ToLong(name.Substring(2, name.Length - 3));
					var channel = _channels[id];
					if (channel != null)
						query = query.Concat(new Channel[] { channel });
				}
				else if (name[0] == '#' && (type == null || type == ChannelType.Text)) //If we somehow get text starting with # but isn't a mention
				{
					string name2 = name.Substring(1);
					query = query.Concat(server.TextChannels.Where(x => string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase)));
				}
			}

			if (type != null)
				query = query.Where(x => x.Type == type);
			return query;
		}

		/// <summary> Creates a new channel with the provided name and type. </summary>
		public async Task<Channel> CreateChannel(Server server, string name, ChannelType type)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (type == null) throw new ArgumentNullException(nameof(type));
			CheckReady();

			var response = await _api.CreateChannel(server.Id, name, type.Value).ConfigureAwait(false);
			var channel = _channels.GetOrAdd(response.Id, response.GuildId, response.Recipient?.Id);
			channel.Update(response);
			return channel;
		}
		
		/// <summary> Returns the private channel with the provided user, creating one if it does not currently exist. </summary>
		public async Task<Channel> CreatePMChannel(User user)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));
			CheckReady();

			Channel channel = null;
			if (user != null)
				channel = user.Global.PrivateChannel;
			if (channel == null)
			{
				var response = await _api.CreatePMChannel(_userId.Value, user.Id).ConfigureAwait(false);
				var recipient = _users.GetOrAdd(response.Recipient.Id, null);
				recipient.Update(response.Recipient);
				channel = _channels.GetOrAdd(response.Id, response.GuildId, response.Recipient.Id);
				channel.Update(response);
			}
			return channel;
		}
		
		/// <summary> Edits the provided channel, changing only non-null attributes. </summary>
		public async Task EditChannel(Channel channel, string name = null, string topic = null, int? position = null)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			CheckReady();

			if (name != null || topic != null)
				await _api.EditChannel(channel.Id, name: name, topic: topic).ConfigureAwait(false);

			if (position != null)
			{
				Channel[] channels = channel.Server.Channels.Where(x => x.Type == channel.Type).OrderBy(x => x.Position).ToArray();
				int oldPos = Array.IndexOf(channels, channel);
				var newPosChannel = channels.Where(x => x.Position > position).FirstOrDefault();
                int newPos = (newPosChannel != null ? Array.IndexOf(channels, newPosChannel) : channels.Length) - 1;
				if (newPos < 0)
					newPos = 0;
				int minPos;

				if (oldPos < newPos) //Moving Down
				{
					minPos = oldPos;
					for (int i = oldPos; i < newPos; i++)
						channels[i] = channels[i + 1];
					channels[newPos] = channel;
				}
				else //(oldPos > newPos) Moving Up
				{
					minPos = newPos;
					for (int i = oldPos; i > newPos; i--)
						channels[i] = channels[i - 1];
					channels[newPos] = channel;
				}
				Channel after = minPos > 0 ? channels.Skip(minPos - 1).FirstOrDefault() : null;
                await ReorderChannels(channel.Server, channels.Skip(minPos), after).ConfigureAwait(false);
			}
		}
		
		/// <summary> Reorders the provided channels in the server's channel list and places them after a certain channel. </summary>
        public Task ReorderChannels(Server server, IEnumerable<Channel> channels, Channel after = null)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (channels == null) throw new ArgumentNullException(nameof(channels));
			CheckReady();

			return _api.ReorderChannels(server.Id, channels.Select(x => x.Id), after?.Position ?? 0);
		}
		
		/// <summary> Destroys the provided channel. </summary>
		public async Task DeleteChannel(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			CheckReady();

			try { await _api.DestroyChannel(channel.Id).ConfigureAwait(false); }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}
	}
}