using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Discord.Models
{
    public class Overwrite
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PermissionTarget TargetType { get; set; }
    }
}
