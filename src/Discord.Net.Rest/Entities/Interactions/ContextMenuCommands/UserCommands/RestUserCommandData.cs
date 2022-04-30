using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommandInteractionData;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents the data for a <see cref="RestUserCommand"/>.
    /// </summary>
    public class RestUserCommandData : RestCommandBaseData, IUserCommandInteractionData, IDiscordInteractionData
    {
        /// <summary>
        ///     Gets the user who this command targets.
        /// </summary>
        public RestUser Member
            => (RestUser)ResolvableData.GuildMembers.Values.FirstOrDefault() ?? ResolvableData.Users.Values.FirstOrDefault();

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>Note</b> Not implemented for <see cref="RestUserCommandData"/>
        /// </remarks>
        public override IReadOnlyCollection<IApplicationCommandInteractionDataOption> Options
            => throw new System.NotImplementedException();

        internal RestUserCommandData(DiscordRestClient client, Model model)
            : base(client, model) { }

        internal new static RestUserCommandData Create(DiscordRestClient client, Model model, RestGuild guild, IRestMessageChannel channel)
        {
            var entity = new RestUserCommandData(client, model);
            entity.Update(client, model, guild, channel);
            return entity;
        }

        //IUserCommandInteractionData
        /// <inheritdoc/>
        IUser IUserCommandInteractionData.User => Member;
    }
}
