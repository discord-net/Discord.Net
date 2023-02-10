using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discord
{
    /// <summary>
    ///     Gets or creates a guild to use for testing.
    /// </summary>
    public class RestGuildFixture : DiscordRestClientFixture
    {
        public RestGuild Guild { get; private set; }

        public RestGuildFixture() : base()
        {
            var guilds = Client.GetGuildsAsync().Result.Where(x => x.OwnerId == Client.CurrentUser.Id).ToList();
            if (guilds.Count == 0)
            {
                // create a new guild if none exists already
                var region = Client.GetOptimalVoiceRegionAsync().Result;
                Guild = Client.CreateGuildAsync("DNET INTEGRATION TEST", region).Result;
                RemoveAllChannels();
            }
            else
            {
                // get the first one if there is a guild already created
                Guild = guilds.First();
            }
        }

        /// <summary>
        ///     Removes all channels in the guild.
        /// </summary>
        private void RemoveAllChannels()
        {
            foreach (var channel in Guild.GetChannelsAsync().Result)
            {
                channel.DeleteAsync().Wait();
            }
        }
    }
}
