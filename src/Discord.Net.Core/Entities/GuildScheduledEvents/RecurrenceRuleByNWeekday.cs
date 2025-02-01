namespace Discord;

public readonly struct RecurrenceRuleByNWeekday
{
    /// <summary>
    ///     Gets the week to reoccur on. (from 1 to 5)
    /// </summary>
    public int Week { get; }

    /// <summary>
    ///     Gets the day within a week to reoccur on.
    /// </summary>
    public RecurrenceRuleWeekday Day { get; }

    internal RecurrenceRuleByNWeekday(int week, RecurrenceRuleWeekday day)
    {
        Week = week;
        Day = day;
    }
}
