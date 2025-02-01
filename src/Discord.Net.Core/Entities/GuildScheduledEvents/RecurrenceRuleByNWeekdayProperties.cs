namespace Discord;

public class RecurrenceRuleByNWeekdayProperties
{
    /// <summary>
    ///     Gets or sets the week to reoccur on. (from 1 to 5)
    /// </summary>
    public int Week { get; set; }

    /// <summary>
    ///     Gets or sets the day within a week to reoccur on.
    /// </summary>
    public RecurrenceRuleWeekday Day { get; set; }

    public RecurrenceRuleByNWeekdayProperties() {}

    public RecurrenceRuleByNWeekdayProperties(int week, RecurrenceRuleWeekday day)
    {
        Week = week;
        Day = day;
    }
}
