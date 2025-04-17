using Newtonsoft.Json;
using System;

namespace Discord.API;

public class GuildScheduledEventRecurrenceRule
{
    [JsonProperty("start")]
    public DateTimeOffset StartAt { get; set; }

    [JsonProperty("end")]
    public DateTimeOffset? EndAt { get; set; }

    [JsonProperty("frequency")]
    public RecurrenceFrequency Frequency { get; set; }

    [JsonProperty("interval")]
    public int Interval { get; set; }

    [JsonProperty("by_weekday")]
    public RecurrenceRuleWeekday[] ByWeekday { get; set; }

    [JsonProperty("by_n_weekday")]
    public GuildScheduledEventRecurrenceRuleByNWeekday[] ByNWeekday { get; set; }

    [JsonProperty("by_month")]
    public RecurrenceRuleMonth[] ByMonth { get; set; }

    [JsonProperty("by_month_day")]
    public int[] ByMonthDay { get; set; }

    [JsonProperty("by_year_day")]
    public int[] ByYearDay { get; set; }

    [JsonProperty("count")]
    public int? Count { get; set; }
}
