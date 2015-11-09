using System;

namespace Discord.Commands.Permissions.Levels
{
	public class PermissionLevelService : IService
	{
		private readonly Func<User, Channel, int> _getPermissionsFunc;

		private DiscordClient _client;
		public DiscordClient Client => _client;

		public PermissionLevelService(Func<User, Channel, int> getPermissionsFunc)
		{
			_getPermissionsFunc = getPermissionsFunc;
        }

		public void Install(DiscordClient client)
		{
			_client = client;
		}
		public int GetPermissionLevel(User user, Channel channel) => _getPermissionsFunc(user, channel);
	}
}
