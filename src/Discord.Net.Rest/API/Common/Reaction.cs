﻿using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class Reaction
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("me")]
        public bool Me { get; set; }
        [JsonPropertyName("emoji")]
        public Emoji Emoji { get; set; }
    }
}
