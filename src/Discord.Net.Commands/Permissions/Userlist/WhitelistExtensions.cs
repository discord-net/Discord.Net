using System.Collections.Generic;

namespace Discord.Commands.Permissions.Userlist
{
    public static class WhitelistExtensions
    {
        public static DiscordClient UsingGlobalWhitelist(this DiscordClient client, params ulong[] initialUserIds)
        {
            client.AddService(new WhitelistService(initialUserIds));
            return client;
        }

        public static CommandBuilder UseGlobalWhitelist(this CommandBuilder builder)
		{
			builder.AddCheck(new WhitelistChecker(builder.Service.Client));
			return builder;
		}
		public static CommandGroupBuilder UseGlobalWhitelist(this CommandGroupBuilder builder)
		{
			builder.AddCheck(new WhitelistChecker(builder.Service.Client));
			return builder;
		}
		public static CommandService UseGlobalWhitelist(this CommandService service)
		{
			service.Root.AddCheck(new BlacklistChecker(service.Client));
			return service;
        }

        public static IEnumerable<ulong> GetWhitelistedUserIds(this DiscordClient client)
            => client.GetService<WhitelistService>().UserIds;
        public static void WhitelistUser(this DiscordClient client, User user)
        {
            client.GetService<WhitelistService>().Add(user.Id);
        }
        public static void WhitelistUser(this DiscordClient client, ulong userId)
        {
            client.GetService<WhitelistService>().Add(userId);
        }
        public static void UnWhitelistUser(this DiscordClient client, User user)
        {
            client.GetService<WhitelistService>().Remove(user.Id);
        }
        public static void RemoveFromWhitelist(this DiscordClient client, ulong userId)
        {
            client.GetService<WhitelistService>().Remove(userId);
        }
    }
}
