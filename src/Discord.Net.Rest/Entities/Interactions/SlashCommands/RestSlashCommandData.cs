using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommandInteractionData;

namespace Discord.Rest
{

    public class RestSlashCommandData : RestCommandBaseData<RestSlashCommandDataOption>, IDiscordInteractionData
    {
        internal RestSlashCommandData(DiscordRestClient client, Model model)
            : base(client, model) { }

        internal static new async Task<RestSlashCommandData> CreateAsync(DiscordRestClient client, Model model, RestGuild guild, IRestMessageChannel channel, bool doApiCall)
        {
            var entity = new RestSlashCommandData(client, model);
            await entity.UpdateAsync(client, model, guild, channel, doApiCall).ConfigureAwait(false);
            return entity;
        }
        internal override async Task UpdateAsync(DiscordRestClient client, Model model, RestGuild guild, IRestMessageChannel channel, bool doApiCall)
        {
            await base.UpdateAsync(client, model, guild, channel, doApiCall).ConfigureAwait(false);

            Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => new RestSlashCommandDataOption(this, x)).ToImmutableArray()
                : ImmutableArray.Create<RestSlashCommandDataOption>();
        }
    }
}
