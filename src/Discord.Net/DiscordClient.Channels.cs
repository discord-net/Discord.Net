using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed class Channels : AsyncCollection<Channel>
	{
		public Channels(DiscordClient client, object writerLock)
			: base(client, writerLock, x => x.OnCached(), x => x.OnUncached()) { }

		public Channel GetOrAdd(string id, string serverId, string recipientId = null)
			=> GetOrAdd(id, () => new Channel(_client, id, serverId, recipientId));
	}

	public class ChannelEventArgs : EventArgs
	{
		public Channel Channel { get; }
		public string ChannelId => Channel.Id;
		public Server Server => Channel.Server;
		public string ServerId => Channel.ServerId;

		internal ChannelEventArgs(Channel channel) { Channel = channel; }
	}

	public partial class DiscordClient
	{
		internal Channels Channels => _channels;
		private readonly Channels _channels;

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


		/// <summary> Returns the channel with the specified id, or null if none was found. </summary>
		public Channel GetChannel(string id) => _channels[id];
		/// <summary> Returns all channels with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and #Name. Search is case-insensitive. </remarks>
		public IEnumerable<Channel> FindChannels(Server server, string name, string type = null) => FindChannels(server?.Id, name, type);
		/// <summary> Returns all channels with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and #Name. Search is case-insensitive. </remarks>
		public IEnumerable<Channel> FindChannels(string serverId, string name, string type = null)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));

			IEnumerable<Channel> result;
			if (name.StartsWith("#"))
			{
				string name2 = name.Substring(1);
				result = _channels.Where(x => x.ServerId == serverId &&
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) ||
					string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase));
			}
			else
			{
				result = _channels.Where(x => x.ServerId == serverId &&
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
			}

			if (type != null)
				result = result.Where(x => x.Type == type);

			return result;
		}

		/// <summary> Creates a new channel with the provided name and type (see ChannelTypes). </summary>
		public Task<Channel> CreateChannel(Server server, string name, string type = ChannelTypes.Text)
			=> CreateChannel(server?.Id, name, type);
		/// <summary> Creates a new channel with the provided name and type (see ChannelTypes). </summary>
		public async Task<Channel> CreateChannel(string serverId, string name, string type = ChannelTypes.Text)
		{
			CheckReady();
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (type == null) throw new ArgumentNullException(nameof(type));

			var response = await _api.CreateChannel(serverId, name, type).ConfigureAwait(false);
			var channel = _channels.GetOrAdd(response.Id, response.GuildId, response.Recipient?.Id);
			channel.Update(response);
			return channel;
		}

		/// <summary> Returns the private channel with the provided user, creating one if it does not currently exist. </summary>
		public Task<Channel> CreatePMChannel(string userId) => CreatePMChannel(_users[userId], userId);
		/// <summary> Returns the private channel with the provided user, creating one if it does not currently exist. </summary>
		public Task<Channel> CreatePMChannel(User user) => CreatePMChannel(user, user?.Id);
		/// <summary> Returns the private channel with the provided user, creating one if it does not currently exist. </summary>
		public Task<Channel> CreatePMChannel(Member member) => CreatePMChannel(member.User, member.UserId);
		private async Task<Channel> CreatePMChannel(User user, string userId)
		{
			CheckReady();
			if (userId == null) throw new ArgumentNullException(nameof(userId));

			Channel channel = null;
			if (user != null)
				channel = user.PrivateChannel;
			if (channel == null)
			{
				var response = await _api.CreatePMChannel(CurrentUserId, userId).ConfigureAwait(false);
				user = _users.GetOrAdd(response.Recipient?.Id);
				user.Update(response.Recipient);
				channel = _channels.GetOrAdd(response.Id, response.GuildId, response.Recipient?.Id);
				channel.Update(response);
			}
			return channel;
		}

		/// <summary> Edits the provided channel, changing only non-null attributes. </summary>
		public Task EditChannel(string channelId, string name = null, string topic = null, int? position = null)
			=> EditChannel(_channels[channelId], name: name, topic: topic, position: position);
		/// <summary> Edits the provided channel, changing only non-null attributes. </summary>
		public async Task EditChannel(Channel channel, string name = null, string topic = null, int? position = null)
		{
			CheckReady();
			if (channel == null) throw new ArgumentNullException(nameof(channel));

			await _api.EditChannel(channel.Id, name: name, topic: topic);

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
				await _api.ReorderChannels(channel.ServerId, channels.Skip(minPos).Select(x => x.Id), minPos);
			}
		}

		public Task ReorderChannels(Server server, IEnumerable<object> channels, int startPos = 0)
			=> ReorderChannels(server.Id, channels, startPos);
		public Task ReorderChannels(string serverId, IEnumerable<object> channels, int startPos = 0)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (channels == null) throw new ArgumentNullException(nameof(channels));
			if (startPos < 0) throw new ArgumentOutOfRangeException(nameof(startPos), "startPos must be a positive integer.");

			var channelIds = CollectionHelper.FlattenChannels(channels);
			return _api.ReorderChannels(serverId, channelIds, startPos);
		}

		/// <summary> Destroys the provided channel. </summary>
		public Task<Channel> DestroyChannel(Channel channel)
			=> DestroyChannel(channel?.Id);
		/// <summary> Destroys the provided channel. </summary>
		public async Task<Channel> DestroyChannel(string channelId)
		{
			CheckReady();
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));

			try { await _api.DestroyChannel(channelId).ConfigureAwait(false); }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
			return _channels.TryRemove(channelId);
		}
	}
}