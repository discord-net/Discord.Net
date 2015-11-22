using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Sessions
{
	public class SessionsService : IService
	{
		private static readonly DualChannelPermissions _ownerPerm = new DualChannelPermissions() { ReadMessages = true, ManageChannel = true };
		private static readonly DualChannelPermissions _memberPerm = new DualChannelPermissions() { ReadMessages = true };
		private static readonly DualChannelPermissions _everyonePerm = new DualChannelPermissions() { ReadMessages = false };

		private DiscordClient _client;

		public void Install(DiscordClient client)
		{
			_client = client;
		}

		public IEnumerable<Channel> GetSessions(Server server)
			=> server.TextChannels.Where(x => x.Name != "" && x.Name[0] == '!');

		public async Task<Channel> CreateSession(Server server, string name, bool includeVoice, User owner)
		{
			name = '!' + name;
			Channel textChannel = await _client.CreateChannel(server, name, ChannelType.Text);
			Channel voiceChannel = includeVoice ? await _client.CreateChannel(server, name, ChannelType.Voice) : null;

			//Take away read from everyone
			await _client.SetChannelPermissions(textChannel, server.EveryoneRole, _everyonePerm);
			await _client.SetChannelPermissions(textChannel, owner, _ownerPerm);

			return textChannel;
		}
		
		public async Task DestroySession(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			CheckSession(channel);

			await _client.DeleteChannel(channel);
		}
		
		public Task JoinSession(Channel channel, User user)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (user == null) throw new ArgumentNullException(nameof(user));
			CheckSession(channel);

			return _client.SetChannelPermissions(channel, user, _memberPerm);
		}
		
		public async Task LeaveSession(Channel channel, User user)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (user == null) throw new ArgumentNullException(nameof(user));
            CheckSession(channel);

			if (IsOwner(channel, user))
				await DestroySession(channel);
			else
				await _client.RemoveChannelPermissions(channel, user);
		}

		private bool IsSession(Channel channel)
			=> channel.Name == "" && channel.Name[0] == '!';
		private void CheckSession(Channel channel)
		{
			if (!IsSession(channel))
				throw new InvalidOperationException("The provided channel is not a session.");
		}
		private bool IsOwner(Channel channel, User user)
			=> _client.GetChannelPermissions(channel, user).ManageMessages == true;
		/*private IEnumerable<string> GetPermissionUsers(Channel channel)
		{
			return channel.PermissionOverwrites
				.Where(x => x.TargetType == PermissionTarget.User && x.Allow.Text_ReadMessages)
				.Select(x => x.TargetId);
		}*/
	}
}
