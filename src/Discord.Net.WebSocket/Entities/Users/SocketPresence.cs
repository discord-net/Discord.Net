using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Model = Discord.IPresenceModel;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the WebSocket user's presence status. This may include their online status and their activity.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketPresence : IPresence, ICached<Model>
    {
        internal ulong UserId;
        internal ulong? GuildId;
        internal bool IsFreed;
        internal DiscordSocketClient Discord;

        /// <inheritdoc />
        public UserStatus Status { get; private set; }
        /// <inheritdoc />
        public IReadOnlyCollection<ClientType> ActiveClients { get; private set; }
        /// <inheritdoc />
        public IReadOnlyCollection<IActivity> Activities { get; private set; }

        public static SocketPresence Default
            => new SocketPresence(null, UserStatus.Offline, null, null);

        internal SocketPresence(DiscordSocketClient discord)
        {
            Discord = discord;
        }
        internal SocketPresence(DiscordSocketClient discord, UserStatus status, IImmutableSet<ClientType> activeClients, IImmutableList<IActivity> activities)
            : this(discord)
        {
            Status = status;
            ActiveClients = activeClients ?? ImmutableHashSet<ClientType>.Empty;
            Activities = activities ?? ImmutableList<IActivity>.Empty;
        }

        internal static SocketPresence Create(DiscordSocketClient client, Model model)
        {
            var entity = new SocketPresence(client);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            Status = model.Status;
            ActiveClients = model.ActiveClients.Length > 0 ? model.ActiveClients.ToImmutableArray() : ImmutableArray<ClientType>.Empty;
            Activities = ConvertActivitiesList(model.Activities) ?? ImmutableArray<IActivity>.Empty;
            UserId = model.UserId;
            GuildId = model.GuildId;
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
        private static IReadOnlyCollection<ClientType> ConvertClientTypesDict(IDictionary<string, string> clientTypesDict)
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
        private static IImmutableList<IActivity> ConvertActivitiesList(IActivityModel[] activities)
        {
            if (activities == null || activities.Length == 0)
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

        internal SocketPresence Clone() => MemberwiseClone() as SocketPresence;

        ~SocketPresence() => Dispose();

        public void Dispose()
        {
            if (IsFreed)
                return;

            GC.SuppressFinalize(this);

            if(Discord != null)
            {
                Discord.StateManager.PresenceStore.RemoveReference(UserId);
                IsFreed = true;
            }
        }

        #region Cache
        private struct CacheModel : Model
        {
            public UserStatus Status { get; set; }

            public ClientType[] ActiveClients { get; set; }

            public IActivityModel[] Activities { get; set; }

            public ulong UserId { get; set; }

            public ulong? GuildId { get; set; }

            ulong IEntityModel<ulong>.Id
            {
                get => UserId;
                set => throw new NotSupportedException();
            }
        }

        private struct ActivityCacheModel : IActivityModel
        {
            public string Id { get; set; }
            public string Url { get; set; }
            public string Name { get; set; }
            public ActivityType Type { get; set; }
            public string Details { get; set; }
            public string State { get; set; }
            public ActivityProperties Flags { get; set; }
            public DateTimeOffset CreatedAt { get; set; }
            public IEmojiModel Emoji { get; set; }
            public ulong? ApplicationId { get; set; }
            public string SyncId { get; set; }
            public string SessionId { get; set; }
            public string LargeImage { get; set; }
            public string LargeText { get; set; }
            public string SmallImage { get; set; }
            public string SmallText { get; set; }
            public string PartyId { get; set; }
            public long[] PartySize { get; set; }
            public string JoinSecret { get; set; }
            public string SpectateSecret { get; set; }
            public string MatchSecret { get; set; }
            public DateTimeOffset? TimestampStart { get; set; }
            public DateTimeOffset? TimestampEnd { get; set; }
        }

        private struct EmojiCacheModel : IEmojiModel
        {
            public ulong? Id { get; set; }
            public string Name { get; set; }
            public ulong[] Roles { get; set; }
            public bool RequireColons { get; set; }
            public bool IsManaged { get; set; }
            public bool IsAnimated { get; set; }
            public bool IsAvailable { get; set; }
            public ulong? CreatorId { get; set; }
        }

        internal Model ToModel()
            => ToModel<CacheModel>();

        internal TModel ToModel<TModel>() where TModel : Model, new()
        {
            return new TModel
            {
                Status = Status,
                ActiveClients = ActiveClients.ToArray(),
                UserId = UserId,
                GuildId = GuildId,
                Activities = Activities.Select(x =>
                {
                    switch (x)
                    {
                        case Game game:
                            switch (game)
                            {
                                case RichGame richGame:
                                    return richGame.ToModel<ActivityCacheModel>();
                                case SpotifyGame spotify:
                                    return spotify.ToModel<ActivityCacheModel>();
                                case CustomStatusGame custom:
                                    return custom.ToModel<ActivityCacheModel, EmojiCacheModel>();
                                case StreamingGame stream:
                                    return stream.ToModel<ActivityCacheModel>();
                            }
                            break;
                    }

                    return new ActivityCacheModel
                    {
                        Name = x.Name,
                        Details = x.Details,
                        Flags = x.Flags,
                        Type = x.Type
                    };
                }).ToArray(),
            };
        }

        Model ICached<Model>.ToModel() => ToModel();
        TResult ICached<Model>.ToModel<TResult>() => ToModel<TResult>();
        void ICached<Model>.Update(Model model) => Update(model);
        bool ICached.IsFreed => IsFreed;

        #endregion
    }
}
