using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed class Channels : AsyncCollection<Channel>
	{
		public IEnumerable<Channel> PrivateChannels => _privateChannels.Select(x => x.Value);
		private ConcurrentDictionary<string, Channel> _privateChannels;

		public Channels(DiscordClient client, object writerLock)
			: base(client, writerLock)
		{
			_privateChannels = new ConcurrentDictionary<string, Channel>();
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

		public Channel GetOrAdd(string id, string serverId, string recipientId = null)
			=> GetOrAdd(id, () => new Channel(_client, id, serverId, recipientId));
	}

	public class ChannelEventArgs : EventArgs
	{
		public Channel Channel { get; }
		public Server Server => Channel.Server;

		internal ChannelEventArgs(Channel channel) { Channel = channel; }
	}

	public partial class DiscordClient
	{
		public event EventHandler<ChannelEventArgs> ChannelCreated;
		private void RaiseChannelCreated(Channel channel)
		{
			if (ChannelCreated != null)
				RaiseEvent(nameof(ChannelCreated), () => ChannelCreated(this, new ChannelEventArgs(channel)));
		}
		public event EventHandler<ChannelEventArgs> ChannelDestroyed;
		private void RaiseChannelDestroyed(Channel channel)
		{
			if (ChannelDestroyed != null)
				RaiseEvent(nameof(ChannelDestroyed), () => ChannelDestroyed(this, new ChannelEventArgs(channel)));
		}
		public event EventHandler<ChannelEventArgs> ChannelUpdated;
		private void RaiseChannelUpdated(Channel channel)
		{
			if (ChannelUpdated != null)
				RaiseEvent(nameof(ChannelUpdated), () => ChannelUpdated(this, new ChannelEventArgs(channel)));
		}

		/// <summary> Returns a collection of all servers this client is a member of. </summary>
		public IEnumerable<Channel> PrivateChannels => _channels.PrivateChannels;
		internal Channels Channels => _channels;
		private readonly Channels _channels;

		/// <summary> Returns the channel with the specified id, or null if none was found. </summary>
		public Channel GetChannel(string id)
		{
			if (id == null) throw new ArgumentNullException(nameof(id));
			CheckReady();

			return _channels[id];
		}
		
		/// <summary> Returns all channels with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and #Name. Search is case-insensitive. </remarks>
		public IEnumerable<Channel> FindChannels(Server server, string name, ChannelType type = null, bool exactMatch = false)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			CheckReady();

			IEnumerable<Channel> result;
			if (!exactMatch && name.StartsWith("#"))
			{
				string name2 = name.Substring(1);
				result = _channels.Where(x => x.Server == server &&
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) ||
					string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase));
			}
			else
			{
				result = _channels.Where(x => x.Server == server &&
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
			}

			if (type != (string)null)
				result = result.Where(x => x.Type == type);

			return result;
		}

		/// <summary> Creates a new channel with the provided name and type. </summary>
		public async Task<Channel> CreateChannel(Server server, string name, ChannelType type)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (type == (string)null) throw new ArgumentNullException(nameof(type));
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
				channel = user.GlobalUser.PrivateChannel;
			if (channel == null)
			{
				var response = await _api.CreatePMChannel(_userId, user.Id).ConfigureAwait(false);
				var recipient = _users.GetOrAdd(response.Recipient?.Id, null);
				recipient.Update(response.Recipient);
				channel = _channels.GetOrAdd(response.Id, response.GuildId, response.Recipient?.Id);
				channel.Update(response);
			}
			return channel;
		}
		
		/// <summary> Edits the provided channel, changing only non-null attributes. </summary>
		public async Task EditChannel(Channel channel, string name = null, string topic = null, int? position = null)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			CheckReady();

			await _api.EditChannel(channel.Id, name: name, topic: topic).ConfigureAwait(false);

			if (position != null)
			{
				int oldPos = channel.Position;
				int newPos = position.Value;
				int minPos;
				Channel[] channels = channel.Server.Channels.OrderBy(x => x.Position).ToArray();

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

			return _api.ReorderChannels(server.Id, channels.Select(x => x.Id), after.Position);
		}
		
		/// <summary> Destroys the provided channel. </summary>
		public async Task DeleteChannel(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			CheckReady();

			try { await _api.DestroyChannel(channel.Id).ConfigureAwait(false); }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
			//return _channels.TryRemove(channel.Id);
		}
	}
}