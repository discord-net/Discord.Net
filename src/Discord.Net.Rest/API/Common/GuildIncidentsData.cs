using Newtonsoft.Json;
using System;

namespace Discord.API;

internal class GuildIncidentsData
{
    [JsonProperty("invites_disabled_until")]
    public DateTimeOffset? InvitesDisabledUntil { get; set; }

    [JsonProperty("dms_disabled_until")]
    public DateTimeOffset? DmsDisabledUntil { get; set; }
}
