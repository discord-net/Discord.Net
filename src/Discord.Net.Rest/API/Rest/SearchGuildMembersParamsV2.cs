using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.Rest;

internal class SearchGuildMembersParamsV2
{
    [JsonProperty("limit")]
    public Optional<int?> Limit { get; set; }

    [JsonProperty("and_query")]
    public Optional<MemberSearchFilter> AndQuery { get; set; }

    [JsonProperty("or_query")]
    public Optional<MemberSearchFilter> OrQuery { get; set; }

    [JsonProperty("after")]
    public Optional<MemberSearchPaginationFilter> After { get; set; }

    [JsonProperty("before")]
    public Optional<MemberSearchPaginationFilter> Before { get; set; }

    [JsonProperty("sort")]
    public Optional<MemberSearchV2SortType> Sort { get; set; }
}

internal class MemberSearchPaginationFilter
{
    [JsonProperty("guild_joined_at")]
    public long GuildJoinedAt { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }
}

internal class MemberSearchFilter
{
    [JsonProperty("safety_signals")]
    public Optional<SafetySignalsProperties> SafetySignals { get; set; }

    [JsonProperty("role_ids")]
    public Optional<SearchQueryProperties> RoleIds { get; set; }

    [JsonProperty("user_id")]
    public Optional<SearchQueryProperties> UserId { get; set; }

    [JsonProperty("guild_joined_at")]
    public Optional<SearchQueryProperties> GuildJoinedAt { get; set; }

    [JsonProperty("source_invite_code")]
    public Optional<SearchQueryProperties> SourceInviteCode { get; set; }

    [JsonProperty("join_source_type")]
    public Optional<SearchQueryProperties> JoinSourceType { get; set; }

    [JsonProperty("did_rejoin")]
    public Optional<bool> DidRejoin { get; set; }

    [JsonProperty("is_pending")]
    public Optional<bool> IsPending { get; set; }

    [JsonProperty("usernames")]
    public Optional<SearchQueryProperties> Usernames { get; set; }
}

internal class SearchQueryProperties
{
    [JsonProperty("and_query")]
    public Optional<IEnumerable<object>> AndQuery { get; set; }

    [JsonProperty("or_query")]
    public Optional<IEnumerable<object>> OrQuery { get; set; }

    [JsonProperty("range")]
    public Optional<SearchRangeProperties> Range { get; set; }
}

internal class SafetySignalsProperties
{
    [JsonProperty("unusual_dm_activity_until")]
    public Optional<SearchQueryProperties> UnusualDMActivityUntil { get; set; }

    [JsonProperty("communication_disabled_until")]
    public Optional<SearchQueryProperties> CommunicationDisabledUntil { get; set; }

    [JsonProperty("unusual_account_activity")]
    public Optional<bool> UnusualAccountActivity { get; set; }

    [JsonProperty("automod_quarantined_username")]
    public Optional<bool> AutomodQuarantinedUsername  { get; set; }
}

internal class SearchRangeProperties
{
    [JsonProperty("gte")]
    public Optional<long> GreaterThanOrEqual { get; set; }

    [JsonProperty("lte")]
    public Optional<long> LessThanOrEqual { get; set; }
}
