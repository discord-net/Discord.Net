using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Model = Discord.API.Presence;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the WebSocket user's presence status. This may include their online status and their activity.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct SocketPresence : IPresence
    {
        /// <inheritdoc />
        public UserStatus Status { get; }
        /// <inheritdoc />
        public IImmutableSet<ClientType> ActiveClients { get; }
        /// <inheritdoc />
        public IImmutableList<IActivity> Activities { get; }
        internal SocketPresence(UserStatus status, IImmutableSet<ClientType> activeClients, IImmutableList<IActivity> activities)
        {
            Status = status;
            ActiveClients = activeClients ?? ImmutableHashSet<ClientType>.Empty;
            Activities = activities ?? ImmutableList<IActivity>.Empty;
        }
        internal static SocketPresence Create(Model model)
        {
            var clients = ConvertClientTypesDict(model.ClientStatus.GetValueOrDefault());
            var activities = ConvertActivitiesList(model.Activities);
            return new SocketPresence(model.Status, clients, activities);
        }
        /// <summary>
        ///     Creates a new <see cref="IReadOnlyCollection{T}"/> containing all of the client types
        ///     where a user is active from the data supplied in the Presence update frame.
        /// </summary>
        /// <param name="clientTypesDict">
        ///     A dictionary keyed by the <see cref="ClientType"/>
        ///     and where the value is the <see cref="UserStatus"/>.
        /// </param>
        /// <returns>
        ///     A collection of all <see cref="ClientType"/>s that this user is active.
        /// </returns>
        private static IImmutableSet<ClientType> ConvertClientTypesDict(IDictionary<string, string> clientTypesDict)
        {
            if (clientTypesDict == null || clientTypesDict.Count == 0)
                return ImmutableHashSet<ClientType>.Empty;
            var set = new HashSet<ClientType>();
            foreach (var key in clientTypesDict.Keys)
            {
                if (Enum.TryParse(key, true, out ClientType type))
                    set.Add(type);
                // quietly discard ClientTypes that do not match
            }
            return set.ToImmutableHashSet();
        }
        /// <summary>
        ///     Creates a new <see cref="IReadOnlyCollection{T}"/> containing all the activities
        ///     that a user has from the data supplied in the Presence update frame.
        /// </summary>
        /// <param name="activities">
        ///     A list of <see cref="API.Game"/>.
        /// </param>
        /// <returns>
        ///     A list of all <see cref="IActivity"/> that this user currently has available.
        /// </returns>
        private static IImmutableList<IActivity> ConvertActivitiesList(IList<API.Game> activities)
        {
            if (activities == null || activities.Count == 0)
                return ImmutableList<IActivity>.Empty;
            var list = new List<IActivity>();
            foreach (var activity in activities)
                list.Add(activity.ToEntity());
            return list.ToImmutableList();
        }

        /// <summary>
        ///     Gets the status of the user.
        /// </summary>
        /// <returns>
        ///     A string that resolves to <see cref="Discord.WebSocket.SocketPresence.Status" />.
        /// </returns>
        public override string ToString() => Status.ToString();
        private string DebuggerDisplay => $"{Status}{(Activities?.FirstOrDefault()?.Name ?? "")}";

        internal SocketPresence Clone() => this;
    }
}
