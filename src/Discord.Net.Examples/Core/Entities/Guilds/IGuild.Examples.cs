using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Discord.Net.Examples.Core.Entities.Guilds
{
    [PublicAPI]
    internal class GuildExamples
    {
        #region CreateTextChannelAsync
        public async Task CreateTextChannelUnderWumpus(IGuild guild, string name)
        {
            var categories = await guild.GetCategoriesAsync();
            var targetCategory = categories.FirstOrDefault(x => x.Name == "wumpus");
            if (targetCategory == null) return;
            await guild.CreateTextChannelAsync(name, x =>
            {
                x.CategoryId = targetCategory.Id;
                x.Topic = $"This channel was created at {DateTimeOffset.UtcNow}.";
            });
        }
        #endregion
    }
}
