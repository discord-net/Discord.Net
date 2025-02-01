using Newtonsoft.Json;

namespace Discord.API;

public class GuildScheduledEventRecurrenceRuleByNWeekday
{
    [JsonProperty("n")]
    public int WeekNumber { get; set; }

    [JsonProperty("day")]
    public RecurrenceRuleWeekday Day { get; set; }
}
