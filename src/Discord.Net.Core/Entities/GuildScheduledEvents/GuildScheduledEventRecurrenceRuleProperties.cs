using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord;

public class GuildScheduledEventRecurrenceRuleProperties
{
    /// <summary>
    ///     Gets or sets the starting time of the recurrence interval.
    /// </summary>
    public DateTimeOffset StartsAt { get; set; }

    /// <summary>
    ///     Gets or sets how often the event occurs.
    /// </summary>
    public RecurrenceFrequency Frequency { get; set; }

    /// <summary>
    ///     Gets or sets the spacing between the events, defined by <see cref="Frequency"/>. 
    /// </summary>
    public int Interval { get; set; }

    /// <summary>
    ///     Gets or sets the set of specific days within a week for the event to recur on.
    /// </summary>
    public HashSet<RecurrenceRuleWeekday> ByWeekday { get; set; }

    /// <summary>
    ///     Gets or sets the list of specific days within a specific week to recur on.
    /// </summary>
    public List<RecurrenceRuleByNWeekday> ByNWeekday { get; set; }

    /// <summary>
    ///     Gets or sets the set of specific months to recur on.
    /// </summary>
    public HashSet<RecurrenceRuleMonth> ByMonth { get; set; }

    /// <summary>
    ///     Gets or sets the set of specific dates within a month to recur on.
    /// </summary>
    public HashSet<int> ByMonthDay { get; set; }


    public GuildScheduledEventRecurrenceRuleProperties() {}

    public GuildScheduledEventRecurrenceRuleProperties(DateTimeOffset startsAt, RecurrenceFrequency frequency,
        int interval, HashSet<RecurrenceRuleWeekday> byWeekday, IEnumerable<RecurrenceRuleByNWeekday> byNWeekday,
        HashSet<RecurrenceRuleMonth> byMonth, HashSet<int> byMonthDay)
    {
        StartsAt = startsAt;
        Frequency = frequency;
        Interval = interval;
        ByWeekday = byWeekday;
        ByNWeekday = byNWeekday?.ToList();
        ByMonth = byMonth;
        ByMonthDay = byMonthDay;
    }
}
