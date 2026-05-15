namespace Discord
{
    /// <summary>
    ///     Represents guild message search filter options.
    ///     Uses the following endpoint: https://docs.discord.com/developers/resources/message#search-guild-messages
    /// </summary>
    public class GetGuildMessagesParams
    {
        /// <summary>
        /// How many messages to fetch.
        /// </summary>
        public Optional<int> Limit { get; set; }

        /// <summary>
        /// How many messages to offset by.
        /// </summary>
        public Optional<int> Offset { get; set; }

        /// <summary>
        /// The minimum message ID that can be returned in the response.
        /// <see cref="SnowflakeUtils.ToSnowflake"/> can be used if you do not have a message id.
        /// </summary>
        public Optional<ulong> MinMessageId { get; set; }

        /// <summary>
        /// The maximum message ID that can be returned in the response.
        /// <see cref="SnowflakeUtils.ToSnowflake"/> can be used if you do not have a message id.
        /// </summary>
        public Optional<ulong> MaxMessageId { get; set; }

        /// <summary>
        /// The slop amount. Dictates the maximum number of words to skip between matching tokens in the <see cref="Content"/>.
        /// </summary>
        public Optional<int> Slop { get; set; }

        /// <summary>
        /// The message content to search for.
        /// </summary>
        public Optional<string> Content { get; set; }

        /// <summary>
        /// A list of channel IDs serving as a whitelist for the search.
        /// </summary>
        public Optional<ulong[]> ChannelIds { get; set; }

        /// <summary>
        /// Whether to whitelist messages sent by regular users.
        /// </summary>
        /// <remarks>
        /// If none of <see cref="AuthorFilterUsers"/>, <see cref="AuthorFilterBots"/> or <see cref="AuthorFilterWebhooks"/> are enabled, the author type of the message will be ignored in the search.
        /// </remarks>
        public Optional<bool> AuthorFilterUsers { get; set; }

        /// <summary>
        /// Whether to whitelist messages sent by bots.
        /// </summary>
        /// <remarks>
        /// If none of <see cref="AuthorFilterUsers"/>, <see cref="AuthorFilterBots"/> or <see cref="AuthorFilterWebhooks"/> are enabled, the author type of the message will be ignored in the search.
        /// </remarks>
        public Optional<bool> AuthorFilterBots { get; set; }

        /// <summary>
        /// Whether to whitelist messages sent using webhooks.
        /// </summary>
        /// <remarks>
        /// If none of <see cref="AuthorFilterUsers"/>, <see cref="AuthorFilterBots"/> or <see cref="AuthorFilterWebhooks"/> are enabled, the author type of the message will be ignored in the search.
        /// </remarks>
        public Optional<bool> AuthorFilterWebhooks { get; set; }

        /// <summary>
        /// A list of user IDs used as a filter for the messages' authors.
        /// </summary>
        public Optional<ulong[]> AuthorIds { get; set; }

        /// <summary>
        /// A list of user IDs that need to be mentioned in the response.
        /// </summary>
        public Optional<ulong[]> UserMentionIds { get; set; }

        /// <summary>
        /// A list of role IDs that need to be mentioned in the response.
        /// </summary>
        public Optional<ulong[]> RoleMentionIds { get; set; }

        /// <summary>
        /// Whether the @everyone mention needs to be present in the message for it to be included in the response.
        /// </summary>
        public Optional<bool> EveryoneMention { get; set; }

        /// <summary>
        /// A list of user IDs that is used as a whitelist checking whether a message replies to any of the specified users.
        /// </summary>
        public Optional<ulong[]> RepliedToUserIds { get; set; }

        /// <summary>
        /// A list of message IDs that is used as a whitelist checking whether a message replies to any of the specified messages.
        /// </summary>
        public Optional<ulong[]> RepliedToMessageIds { get; set; }

        /// <summary>
        /// Whether the message needs to be pinned for it to be included in the response.
        /// </summary>
        public Optional<bool> IsPinned { get; set; }

        /// <summary>
        /// The slop amount. Dictates the maximum number of words to skip between matching tokens in the <see cref="Content"/>.
        /// </summary>
        public Optional<SortDirection> SortDirection { get; set; }

        /// <summary>
        /// The slop amount. Dictates the maximum number of words to skip between matching tokens in the <see cref="Content"/>.
        /// </summary>
        public Optional<SortAlgorithm> SortAlgorithm { get; set; }

        /// <summary>
        /// Whether to include messages from NSFW channels in the response.
        /// </summary>
        public Optional<bool> IncludeNsfw { get; set; }
    }
}
