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

        internal static new RestSlashCommandData Create(DiscordRestClient client, Model model, RestGuild guild, IRestMessageChannel channel)
        {
            var entity = new RestSlashCommandData(client, model);
            entity.Update(client, model, guild, channel);
            return entity;
        }
        internal override void Update(DiscordRestClient client, Model model, RestGuild guild, IRestMessageChannel channel)
        {
            base.Update(client, model, guild, channel);

            Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => new RestSlashCommandDataOption(this, x)).ToImmutableArray()
                : ImmutableArray.Create<RestSlashCommandDataOption>();
        }
    }
}
