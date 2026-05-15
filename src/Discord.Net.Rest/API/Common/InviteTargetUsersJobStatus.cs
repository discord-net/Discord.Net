using Newtonsoft.Json;
using System;

namespace Discord.API;

internal class InviteTargetUsersJobStatus
{
    [JsonProperty("status")]
    public TargetUsersStatusCode Status { get; set; }

    [JsonProperty("total_users")]
    public int TotalUsers { get; set; }

    [JsonProperty("processed_users")]
    public int ProcessedUsers { get; set; }

    [JsonProperty("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonProperty("completed_at")]
    public DateTimeOffset? CompletedAt { get; set; }

    [JsonProperty("error_message")]
    public string ErrorMessage { get; set; }
}
