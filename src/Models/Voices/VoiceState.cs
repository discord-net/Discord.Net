using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord voice state object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/voice#voice-state-object-voice-state-structure"/>
    /// </remarks>
    public record VoiceState
    {
        /// <summary>
        /// The <see cref="Guild"/> id this <see cref="VoiceState"/> is for.
        /// </summary>
        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; init; }

        /// <summary>
        /// The <see cref="Channel"/> id this <see cref="User"/> is connected to.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Snowflake? ChannelId { get; init; }

        /// <summary>
        /// The <see cref="User"/> id this <see cref="VoiceState"/> is for.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Snowflake UserId { get; init; }

        /// <summary>
        /// The <see cref="GuildMember"/> this <see cref="VoiceState"/> is for.
        /// </summary>
        [JsonPropertyName("member")]
        public Optional<GuildMember> Member { get; init; }

        /// <summary>
        /// The session id for this <see cref="VoiceState"/>.
        /// </summary>
        [JsonPropertyName("session_id")]
        public string? SessionId { get; init; } // Required property candidate

        /// <summary>
        /// Whether this <see cref="User"/> is deafened by the server.
        /// </summary>
        [JsonPropertyName("deaf")]
        public bool Deaf { get; init; }

        /// <summary>
        /// Whether this <see cref="User"/> is muted by the server.
        /// </summary>
        [JsonPropertyName("mute")]
        public bool Mute { get; init; }

        /// <summary>
        /// Whether this <see cref="User"/> is locally deafened.
        /// </summary>
        [JsonPropertyName("self_deaf")]
        public bool SelfDeaf { get; init; }

        /// <summary>
        /// Whether this <see cref="User"/> is locally muted.
        /// </summary>
        [JsonPropertyName("self_mute")]
        public bool SelfMute { get; init; }

        /// <summary>
        /// Whether this <see cref="User"/> is streaming using "Go Live".
        /// </summary>
        [JsonPropertyName("self_stream")]
        public Optional<bool> SelfStream { get; init; }

        /// <summary>
        /// Whether this <see cref="User"/>'s camera is enabled.
        /// </summary>
        [JsonPropertyName("self_video")]
        public bool SelfVideo { get; init; }

        /// <summary>
        /// Whether this <see cref="User"/> is muted by the current <see cref="User"/>.
        /// </summary>
        [JsonPropertyName("suppress")]
        public bool Suppress { get; init; }

        /// <summary>
        /// The time at which the <see cref="User"/> requested to speak.
        /// </summary>
        [JsonPropertyName("request_to_speak_timestamp")]
        public DateTimeOffset? RequestToSpeakTimestamp { get; init; }
    }
}
