using System;
using System.Threading.Tasks;
using Discord.Rest;

namespace Discord
{
    public partial class TestsFixture
    {
        public const uint MigrationCount = 3;

        public async Task MigrateAsync()
        {
            DiscordRestClient client = null;
            RestGuild guild = null;

            await _cache.LoadInfoAsync(_config.GuildId).ConfigureAwait(false);
            while (_cache.Info.Version != MigrationCount)
            {
                if (client == null)
                {
                    client = new DiscordRestClient();
                    await client.LoginAsync(TokenType.Bot, _config.Token, false).ConfigureAwait(false);
                    guild = await client.GetGuildAsync(_config.GuildId);
                }

                uint nextVer = _cache.Info.Version + 1;
                try
                {
                    await DoMigrateAsync(client, guild, nextVer).ConfigureAwait(false);
                    _cache.Info.Version = nextVer;
                    await _cache.SaveInfoAsync().ConfigureAwait(false);
                }
                catch
                {
                    await _cache.ClearAsync().ConfigureAwait(false);
                    throw;
                }
            }
        }

        private static Task DoMigrateAsync(DiscordRestClient client, RestGuild guild, uint toVersion)
        {
            switch (toVersion)
            {
                case 1: return Migration_WipeGuild(client, guild);
                case 2: return Tests.Migration_CreateTextChannels(client, guild);
                case 3: return Tests.Migration_CreateVoiceChannels(client, guild);
                default: throw new InvalidOperationException("Unknown migration: " + toVersion);
            }
        }

        private static async Task Migration_WipeGuild(DiscordRestClient client, RestGuild guild)
        {
            var textChannels = await guild.GetTextChannelsAsync();
            var voiceChannels = await guild.GetVoiceChannelsAsync();
            var roles = guild.Roles;
            
            foreach (var channel in textChannels)
            {
                if (channel.Id != guild.DefaultChannelId)
                    await channel.DeleteAsync();
            }
            foreach (var channel in voiceChannels)
                await channel.DeleteAsync();
            foreach (var role in roles)
            {
                if (role.Id != guild.EveryoneRole.Id)
                    await role.DeleteAsync();
            }
        }
    }
}