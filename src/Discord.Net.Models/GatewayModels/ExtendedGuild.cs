using Discord.Models;
using Discord.Models.Json;
using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal sealed class ExtendedGuild : Guild
    {
        [JsonPropertyName("unavailable")]
        public bool? Unavailable { get; set; }

        [JsonPropertyName("member_count")]
        public int MemberCount { get; set; }

        [JsonPropertyName("large")]
        public bool Large { get; set; }

        [JsonPropertyName("presences")]
        public Presence[]? Presences { get; set; }

        [JsonPropertyName("members")]
        public GuildMember[]? Members { get; set; }

        [JsonPropertyName("channels")]
        public Channel[]? Channels { get; set; }

        [JsonPropertyName("joined_at")]
        public DateTimeOffset JoinedAt { get; set; }

        [JsonPropertyName("guild_scheduled_events")]
        public GuildScheduledEvent[]? GuildScheduledEvents { get; set; }

        [JsonPropertyName("stage_instances")]
        public StageInstance[]? StageInstances { get; set; }

        public override IEnumerable<IEntityModel> GetEntities()
        {
            var entities = base.GetEntities();

            if (Presences is not null)
                entities = entities.Concat(Presences);

            if (Members is not null)
                entities = entities.Concat(Members);

            if (Channels is not null)
                entities = entities.Concat(Channels);

            if (GuildScheduledEvents is not null)
                entities = entities.Concat(GuildScheduledEvents);

            if (StageInstances is not null)
                entities = entities.Concat(StageInstances);

            return entities;
        }
    }
}

