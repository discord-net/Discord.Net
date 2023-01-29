using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord;

/// <summary>
///     A builder used to create a <see cref="IAutoModRule"/>.
/// </summary>
public class AutoModRuleBuilder
{
    private const int MaxKeywordCount = 1000;
    private const int MaxKeywordLength = 30;

    private const int MaxRegexPatternCount = 10;
    private const int MaxRegexPatternLength = 260;

    private const int MaxAllowListCountKeyword = 100;
    private const int MaxAllowListCountKeywordPreset = 1000;
    private const int MaxAllowListEntryLength = 30;

    private const int MaxMentionLimit = 50;

    private const int MaxExemptRoles = 20;
    private const int MaxExemptChannels = 50;

    private List<string> _keywordFilter = new ();
    private List<string> _regexPatterns = new ();
    private List<string> _allowList = new ();
    private List<AutoModRuleActionBuilder> _actions = new ();

    private List<ulong> _exemptRoles = new();
    private List<ulong> _exemptChannels = new();

    private int? _mentionLimit;

    /// <summary>
    ///     
    /// </summary>
    public List<string> KeywordFilter
    {
        get { return _keywordFilter; }
        set
        {
            if (TriggerType != AutoModTriggerType.Keyword)
                throw new ArgumentException(message: $"Keyword filter can only be used with 'Keyword' trigger type.", paramName: nameof(KeywordFilter));

            if (value.Count > MaxKeywordCount)
                throw new ArgumentException(message: $"Keyword count must be less than or equal to {MaxKeywordCount}.", paramName: nameof(KeywordFilter));

            if (value.Any(x => x.Length > MaxKeywordLength))
                throw new ArgumentException(message: $"Keyword length must be less than or equal to {MaxKeywordLength}.", paramName: nameof(KeywordFilter));

            _keywordFilter = value;
        }
    }

    /// <summary>
    ///     
    /// </summary>
    public List<string> RegexPatterns
    {
        get { return _regexPatterns; }
        set
        {
            if (TriggerType != AutoModTriggerType.Keyword)
                throw new ArgumentException(message: $"Regex patterns can only be used with 'Keyword' trigger type.", paramName: nameof(RegexPatterns));

            if (value.Count > MaxRegexPatternCount)
                throw new ArgumentException(message: $"Regex pattern count must be less than or equal to {MaxRegexPatternCount}.", paramName: nameof(RegexPatterns));

            if (value.Any(x => x.Length > MaxRegexPatternLength))
                throw new ArgumentException(message: $"Regex pattern must be less than or equal to {MaxRegexPatternLength}.", paramName: nameof(RegexPatterns));

            _regexPatterns = value;
        }
    }

    /// <summary>
    ///     
    /// </summary>
    public List<string> AllowList
    {
        get { return _allowList; }
        set
        {
            if (TriggerType is not AutoModTriggerType.Keyword or AutoModTriggerType.KeywordPreset)
                throw new ArgumentException(message: $"Allow list can only be used with 'Keyword' or 'KeywordPreset' trigger type.", paramName: nameof(AllowList));

            if (TriggerType == AutoModTriggerType.Keyword && value.Count > MaxAllowListCountKeyword)
                throw new ArgumentException(message: $"Allow list entry count must be less than or equal to {MaxAllowListCountKeyword}.", paramName: nameof(AllowList));

            if (TriggerType == AutoModTriggerType.KeywordPreset && value.Count > MaxAllowListCountKeywordPreset)
                throw new ArgumentException(message: $"Allow list entry count must be less than or equal to {MaxAllowListCountKeywordPreset}.", paramName: nameof(AllowList));

            if (value.Any(x => x.Length > MaxAllowListEntryLength))
                throw new ArgumentException(message: $"Allow list entry length must be less than or equal to {MaxAllowListEntryLength}.", paramName: nameof(AllowList));

            _allowList = value;
        }
    }

    /// <summary>
    ///     
    /// </summary>
    public List<ulong> ExemptRoles
    {
        get { return _exemptRoles; }
        set
        {
            if (value.Count > MaxExemptRoles)
                throw new ArgumentException(message: $"Exempt roles count must be less than or equal to {MaxExemptRoles}.", paramName: nameof(RegexPatterns));

            _exemptRoles = value;
        }
    }

    /// <summary>
    ///     
    /// </summary>
    public List<ulong> ExemptChannels
    {
        get { return _exemptChannels; }
        set
        {
            if (value.Count > MaxExemptRoles)
                throw new ArgumentException(message: $"Exempt channels count must be less than or equal to {MaxExemptRoles}.", paramName: nameof(RegexPatterns));

            _exemptChannels = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public HashSet<KeywordPresetTypes> Presets = new ();

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public AutoModEventType EventType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public AutoModTriggerType TriggerType { get; }

    /// <summary>
    /// 
    /// </summary>
    public int? MentionLimit
    {
        get => _mentionLimit;
        set
        {
            if (TriggerType != AutoModTriggerType.Keyword)
                throw new ArgumentException(message: $"MentionLimit can only be used with 'MentionSpam' trigger type.", paramName: nameof(MentionLimit));

            if (value is not null && value > MaxMentionLimit)
                throw new ArgumentException(message: $"Mention limit must be less than or equal to {MaxMentionLimit}.", paramName: nameof(MentionLimit));
            _mentionLimit = value;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public List<AutoModRuleActionBuilder> Actions;

    /// <summary>
    /// 
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    public AutoModRuleBuilder(AutoModTriggerType type)
    {
        TriggerType = type;
    }
}

/// <summary>
///     Represents an action that will be preformed if a user breaks an <see cref="IAutoModRule"/>.
/// </summary>
public class AutoModRuleActionBuilder
{
    private const int MaxTimeoutSeconds = 2419200;

    private TimeSpan? _timeoutDuration;

    /// <summary>
    ///     Gets or sets the type for this action.
    /// </summary>
    public AutoModActionType Type { get; }

    /// <summary>
    ///     Get or sets the channel id on which to post alerts.
    /// </summary>
    public ulong? ChannelId { get; set; }

    /// <summary>
    ///     Gets or sets the duration of which a user will be timed out for breaking this rule.
    /// </summary>
    public TimeSpan? TimeoutDuration
    {
        get => _timeoutDuration;
        set
        {
            if(value is { TotalSeconds: > MaxTimeoutSeconds })
                throw new ArgumentException(message: $"Field count must be less than or equal to {MaxTimeoutSeconds}.", paramName: nameof(TimeoutDuration));

            _timeoutDuration = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public AutoModRuleActionBuilder()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public AutoModRuleActionBuilder(AutoModActionType type, ulong? channelId, TimeSpan? duration)
    {
        Type = type;
        ChannelId = channelId;
        TimeoutDuration = duration;
    }
}
