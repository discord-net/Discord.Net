using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a partial <see cref="IDiscordInteraction"/> within a message.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    public class MessageInteraction<TUser> : IMessageInteraction where TUser : IUser
    {
        /// <summary>
        ///     Gets the snowflake id of the interaction.
        /// </summary>
        public ulong Id { get; }

        /// <summary>
        ///     Gets the type of the interaction.
        /// </summary>
        public InteractionType Type { get; }

        /// <summary>
        ///     Gets the name of the application command used.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the <typeparamref name="TUser"/> who invoked the interaction.
        /// </summary>
        public TUser User { get; }

        internal MessageInteraction(ulong id, InteractionType type, string name, TUser user)
        {
            Id = id;
            Type = type;
            Name = name;
            User = user;
        }

        IUser IMessageInteraction.User => User;
    }
}
