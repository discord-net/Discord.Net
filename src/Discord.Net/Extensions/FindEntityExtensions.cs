using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Extensions
{
    public static class FindEntityExtensions
    {
        // Guilds

        public static async Task<IEnumerable<IGuildUser>> FindUsersAsync(this IGuild guild, string name)
            => (await guild.GetUsersAsync()).Where(x => distance(name, x.Username) < 5);

        public static IEnumerable<IRole> FindRoles(this IGuild guild, string name)
            => guild.Roles.Where(x => distance(name, x.Name) < 5);

        public static async Task<IEnumerable<IGuildChannel>> FindChannels(this IGuild guild, string name)
            => (await guild.GetChannelsAsync()).Where(x => distance(name, x.Name) < 5);

        public static async Task<IEnumerable<ITextChannel>> FindTextChannels(this IGuild guild, string name)
            => (await guild.GetChannelsAsync()).Select(x => x as ITextChannel)
                .Where(x => x != null).Where(x => distance(name, x.Name) < 5);

        public static async Task<IEnumerable<IVoiceChannel>> FindVoiceChannels(this IGuild guild, string name)
            => (await guild.GetChannelsAsync()).Select(x => x as IVoiceChannel)
                .Where(x => x != null).Where(x => distance(name, x.Name) < 5);

        // Channels

        public static async Task<IEnumerable<IUser>> FindUsersAsync(this IChannel channel, string name)
            => (await channel.GetUsersAsync()).Where(x => distance(name, x.Username) < 5);

        /// <summary>
        /// Compute the distance between two strings.
        /// Copied from DotNetPerls
        /// </summary>
        private static int distance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
                return m;

            if (m == 0)
                return n;

            for (int i = 0; i <= n; d[i, 0] = i++) { }

            for (int j = 0; j <= m; d[0, j] = j++) { }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }
    }
}
