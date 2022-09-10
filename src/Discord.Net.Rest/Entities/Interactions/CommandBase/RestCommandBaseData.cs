using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommandInteractionData;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents the base data tied with the <see cref="RestCommandBase"/> interaction.
    /// </summary>
    public class RestCommandBaseData<TOption> : RestEntity<ulong>, IApplicationCommandInteractionData where TOption : IApplicationCommandInteractionDataOption
    {
        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets a collection of <typeparamref name="TOption"/> received with this interaction.
        /// </summary>
        public virtual IReadOnlyCollection<TOption> Options { get; internal set; }

        internal RestResolvableData<Model> ResolvableData;

        internal RestCommandBaseData(BaseDiscordClient client, Model model)
            : base(client, model.Id)
        {
        }

        internal static async Task<RestCommandBaseData> CreateAsync(DiscordRestClient client, Model model, RestGuild guild, IRestMessageChannel channel, bool doApiCall)
        {
            var entity = new RestCommandBaseData(client, model);
            await entity.UpdateAsync(client, model, guild, channel, doApiCall).ConfigureAwait(false);
            return entity;
        }

        internal virtual async Task UpdateAsync(DiscordRestClient client, Model model, RestGuild guild, IRestMessageChannel channel, bool doApiCall)
        {
            Name = model.Name;
            if (model.Resolved.IsSpecified && ResolvableData == null)
            {
                ResolvableData = new RestResolvableData<Model>();
                await ResolvableData.PopulateAsync(client, guild, channel, model, doApiCall).ConfigureAwait(false);
            }
        }

        IReadOnlyCollection<IApplicationCommandInteractionDataOption> IApplicationCommandInteractionData.Options
            => (IReadOnlyCollection<IApplicationCommandInteractionDataOption>)Options;
    }

    /// <summary>
    ///     Represents the base data tied with the <see cref="RestCommandBase"/> interaction.
    /// </summary>
    public class RestCommandBaseData : RestCommandBaseData<IApplicationCommandInteractionDataOption>
    {
        internal RestCommandBaseData(DiscordRestClient client, Model model)
            : base(client, model) { }
    }
}
