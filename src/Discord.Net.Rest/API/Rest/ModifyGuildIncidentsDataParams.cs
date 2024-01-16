using Newtonsoft.Json;
using System;

namespace Discord.API.Rest;

internal class ModifyGuildIncidentsDataParams
{
    [JsonProperty("invites_disabled_until")]
    public Optional<DateTimeOffset?> InvitesDisabledUntil { get; set; }

    [JsonProperty("dms_disabled_until")]
    public Optional<DateTimeOffset?> DmsDisabledUntil { get; set; }
}
