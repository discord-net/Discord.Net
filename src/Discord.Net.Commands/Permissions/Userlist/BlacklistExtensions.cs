using System.Collections.Generic;

namespace Discord.Commands.Permissions.Userlist
{
    public static class BlacklistExtensions
    {
        public static DiscordClient UsingGlobalBlacklist(this DiscordClient client, params ulong[] initialUserIds)
        {
            client.AddService(new BlacklistService(initialUserIds));
            return client;
        }

        public static CommandBuilder UseGlobalBlacklist(this CommandBuilder builder)
		{
			builder.AddCheck(new BlacklistChecker(builder.Service.Client));
			return builder;
		}
		public static CommandGroupBuilder UseGlobalBlacklist(this CommandGroupBuilder builder)
		{
			builder.AddCheck(new BlacklistChecker(builder.Service.Client));
			return builder;
		}
		public static CommandService UseGlobalBlacklist(this CommandService service)
		{
			service.Root.AddCheck(new BlacklistChecker(service.Client));
			return service;
		}

        public static IEnumerable<ulong> GetBlacklistedUserIds(this DiscordClient client)
            => client.GetService<BlacklistService>().UserIds;
        public static void BlacklistUser(this DiscordClient client, User user)
        {
            client.GetService<BlacklistService>().Add(user.Id);
        }
        public static void BlacklistUser(this DiscordClient client, ulong userId)
        {
            client.GetService<BlacklistService>().Add(userId);
        }
        public static void UnBlacklistUser(this DiscordClient client, User user)
        {
            client.GetService<BlacklistService>().Remove(user.Id);
        }
        public static void UnBlacklistUser(this DiscordClient client, ulong userId)
        {
            client.GetService<BlacklistService>().Remove(userId);
        }
    }
}
