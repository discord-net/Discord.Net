using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a connection object.
    /// </summary>
    public record Connection
    {
        /// <summary>
        ///     Creates a <see cref="Connection"/> with the provided parameters.
        /// </summary>
        /// <param name="id">Id of the connection account.</param>
        /// <param name="name">The username of the connection account.</param>
        /// <param name="type">The service of the connection (twitch, youtube).</param>
        /// <param name="revoked">Whether the connection is revoked.</param>
        /// <param name="integrations">An array of partial server integrations.</param>
        /// <param name="verified">Whether the connection is verified.</param>
        /// <param name="friendSync">Whether friend sync is enabled for this connection.</param>
        /// <param name="showActivity">Whether activities related to this connection will be shown in presence updates.</param>
        /// <param name="visibility">Visibility of this connection.</param>
        [JsonConstructor]
        public Connection(string id, string name, string type, Optional<bool> revoked, Optional<Integration[]> integrations, bool verified, bool friendSync, bool showActivity, VisibilityType visibility)
        {
            Id = id;
            Name = name;
            Type = type;
            Revoked = revoked;
            Integrations = integrations;
            Verified = verified;
            FriendSync = friendSync;
            ShowActivity = showActivity;
            Visibility = visibility;
        }

        /// <summary>
        ///     Id of the connection account.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; }

        /// <summary>
        ///     The username of the connection account.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        ///     The service of the connection (twitch, youtube).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; }

        /// <summary>
        ///     Whether the connection is revoked.
        /// </summary>
        [JsonPropertyName("revoked")]
        public Optional<bool> Revoked { get; }

        /// <summary>
        ///     An array of partial server integrations.
        /// </summary>
        [JsonPropertyName("integrations")]
        public Optional<Integration[]> Integrations { get; }

        /// <summary>
        ///     Whether the connection is verified.
        /// </summary>
        [JsonPropertyName("verified")]
        public bool Verified { get; }

        /// <summary>
        ///     Whether friend sync is enabled for this connection.
        /// </summary>
        [JsonPropertyName("friend_sync")]
        public bool FriendSync { get; }

        /// <summary>
        ///     Whether activities related to this connection will be shown in presence updates.
        /// </summary>
        [JsonPropertyName("show_activity")]
        public bool ShowActivity { get; }

        /// <summary>
        ///     Visibility of this connection.
        /// </summary>
        [JsonPropertyName("visibility")]
        public VisibilityType Visibility { get; }
    }
}
