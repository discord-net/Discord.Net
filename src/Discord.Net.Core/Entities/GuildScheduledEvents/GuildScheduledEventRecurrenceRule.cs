using System;
using System.Collections.Generic;

namespace Discord;

public readonly struct GuildScheduledEventRecurrenceRule
{
    /// <summary>
    ///     Gets the starting time of the recurrence interval.
    /// </summary>
    public DateTimeOffset StartsAt { get; }

    /// <summary>
    ///     Gets the ending time of the recurrence interval.
    /// </summary>
    public DateTimeOffset? EndsAt { get; }

    /// <summary>
    ///     Gets how often the event occurs.
    /// </summary>
    public RecurrenceFrequency Frequency { get; }

    /// <summary>
    ///     Gets the spacing between the events, defined by <see cref="Frequency"/>. 
    /// </summary>
    public int Interval { get; }

    /// <summary>
    ///     Gets the set of specific days within a week for the event to recur on.
    /// </summary>
    public IReadOnlyCollection<RecurrenceRuleWeekday> ByWeekday { get; }

    /// <summary>
    ///     Gets the list of specific days within a specific week to recur on.
    /// </summary>
    public IReadOnlyCollection<RecurrenceRuleByNWeekday> ByNWeekday { get; }

    /// <summary>
    ///     Gets the set of specific months to recur on.
    /// </summary>
    public IReadOnlyCollection<RecurrenceRuleMonth> ByMonth { get; }

    /// <summary>
    ///     Gets the set of specific dates within a month to recur on.
    /// </summary>
    public IReadOnlyCollection<int> ByMonthDay { get; }

    /// <summary>
    ///     Gets the set of days within a year to recur on. (1-364)
    /// </summary>
    public IReadOnlyCollection<int> ByYearDay { get; }

    /// <summary>
    ///     Gets the total amount of times that the event is allowed to recur before stopping.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if the event recurs endlessly.
    /// </remarks>
    public int? Count { get; }

    internal GuildScheduledEventRecurrenceRule(DateTimeOffset startsAt, DateTimeOffset? endsAt, RecurrenceFrequency frequency,
        int interval, IReadOnlyCollection<RecurrenceRuleWeekday> byWeekday, IReadOnlyCollection<RecurrenceRuleByNWeekday> byNWeekday,
        IReadOnlyCollection<RecurrenceRuleMonth> byMonth, IReadOnlyCollection<int> byMonthDay, IReadOnlyCollection<int> byYearDay, int? count)
    {
        StartsAt = startsAt;
        EndsAt = endsAt;
        Frequency = frequency;
        Interval = interval;
        ByWeekday = byWeekday;
        ByNWeekday = byNWeekday;
        ByMonth = byMonth;
        ByMonthDay = byMonthDay;
        ByYearDay = byYearDay;
        Count = count;
    }
}
