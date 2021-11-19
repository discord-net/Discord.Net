using System.Collections.Generic;
using System.Linq;
using Model = Discord.API.ApplicationCommandInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the data tied with the <see cref="SocketUserCommand"/> interaction.
    /// </summary>
    public class SocketUserCommandData : SocketCommandBaseData, IUserCommandInteractionData, IDiscordInteractionData
    {
        /// <summary>
        ///     Gets the user who this command targets.
        /// </summary>
        public SocketUser Member
            => (SocketUser)ResolvableData.GuildMembers.Values.FirstOrDefault() ?? ResolvableData.Users.Values.FirstOrDefault();

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>Note</b> Not implemented for <see cref="SocketUserCommandData"/>
        /// </remarks>
        public override IReadOnlyCollection<IApplicationCommandInteractionDataOption> Options
            => throw new System.NotImplementedException();

        internal SocketUserCommandData(DiscordSocketClient client, Model model, ulong? guildId)
            : base(client, model, guildId) { }

        internal new static SocketUserCommandData Create(DiscordSocketClient client, Model model, ulong id, ulong? guildId)
        {
            var entity = new SocketUserCommandData(client, model, guildId);
            entity.Update(model);
            return entity;
        }

        //IUserCommandInteractionData
        /// <inheritdoc/>
        IUser IUserCommandInteractionData.User => Member;
    }
}
