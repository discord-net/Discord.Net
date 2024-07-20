using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.Rest;

internal class SearchGuildMembersParamsV2
{
    [JsonProperty("limit")]
    public Optional<int?> Limit { get; set; }

    [JsonProperty("and_query")]
    public Optional<SearchQueryParams> AndQuery { get; set; }

    [JsonProperty("or_query")]
    public Optional<SearchQueryParams> OrQuery { get; set; }

    [JsonProperty("after")]
    public Optional<SearchParamsAfter> After { get; set; }

    [JsonProperty("sort")]
    public Optional<MemberSearchV2SortType> Sort { get; set; }
}

internal class SearchParamsAfter
{
    [JsonProperty("guild_joined_at")]
    public long GuildJoinedAt { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }
}

internal class SearchQueryParams
{
    [JsonProperty("safety_signals")]
    public Optional<SafetySignalsProperties> SafetySignals { get; set; }

    [JsonProperty("role_ids")]
    public Optional<SearchQueryProperties> RoleIds { get; set; }

    [JsonProperty("user_id")]
    public Optional<SearchRangeProperties> UserId { get; set; }

    [JsonProperty("guild_joined_at")]
    public Optional<SearchRangeProperties> GuildJoinedAt { get; set; }

    [JsonProperty("source_invite_code")]
    public Optional<SearchQueryProperties> SourceInviteCode { get; set; }

    [JsonProperty("join_source_type")]
    public Optional<SearchQueryProperties> JoinSourceType { get; set; }
}

internal class SearchQueryProperties
{
    [JsonProperty("and_query")]
    public Optional<Dictionary<int, object>> AndQuery { get; set; }

    [JsonProperty("or_query")]
    public Optional<Dictionary<int, object>> OrQuery { get; set; }
}

internal class SafetySignalsProperties
{
    [JsonProperty("unusual_dm_activity_until")]
    public Optional<SafetySignalProperties> UnusualDMActivityUntil { get; set; }

    [JsonProperty("communication_disabled_until")]
    public Optional<SafetySignalProperties> CommunicationDisabledUntil { get; set; }

    [JsonProperty("unusual_account_activity")]
    public Optional<bool> UnusualAccountActivity { get; set; }

    [JsonProperty("automod_quarantined_username")]
    public Optional<bool> AutomodQuarantinedUsername  { get; set; }
}

internal class SafetySignalProperties
{
    [JsonProperty("range")]
    public SearchRangeProperties Until { get; set; }
}

internal class SearchRangeProperties
{
    [JsonProperty("gte")]
    public Optional<long> GreaterThanOrEqual { get; set; }

    [JsonProperty("lte")]
    public Optional<long> LessThanOrEqual { get; set; }
}
