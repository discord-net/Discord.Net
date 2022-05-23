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
    public class MessageInteraction<TUser> : IMessageInteraction where TUser : class, IUser
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
        /// <remarks>
        ///     When this property is a SocketUser, the get accessor will attempt to preform a
        ///     synchronous cache lookup.
        /// </remarks>
        public TUser User
            => _user ?? (_userLookup != null ? _userLookup(UserId) : null);

        /// <summary>
        ///     Gets the id of the user who invoked the interaction.
        /// </summary>
        public ulong UserId { get; }

        private readonly TUser _user;
        private readonly Func<ulong, TUser> _userLookup;
        internal MessageInteraction(ulong id, InteractionType type, string name, TUser user)
        {
            Id = id;
            Type = type;
            Name = name;
            _user = user;
            UserId = user.Id;
        }

        internal MessageInteraction(ulong id, InteractionType type, string name, ulong userId, Func<ulong, TUser> lookup)
        {
            Id = id;
            Type = type;
            Name = name;
            UserId = userId;
            _userLookup = lookup;
        }

        IUser IMessageInteraction.User => User;
    }
}
