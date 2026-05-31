using System;
using System.Collections.Generic;

namespace Discord;

public class GuildMessageSearchData
{
    /// <summary>
    ///     Gets whether the guild is undergoing a deep historical indexing operation.
    /// </summary>
    public bool DoingDeepHistoricalIndex { get; internal set; }

    /// <summary>
    ///     Gets the number of documents that have been indexed during the current index operation, if any.
    /// </summary>
    public int? DocumentsIndexed { get; internal set; }

    /// <summary>
    ///     Gets the total number of results that match the query.
    /// </summary>
    public int TotalResults { get; internal set; }

    /// <summary>
    ///     Gets an array of messages that match the query
    /// </summary>
    public IReadOnlyCollection<IMessage> Messages { get; internal set; }

    /// <summary>
    ///     Gets the threads that contain the returned messages.
    /// </summary>
    public IReadOnlyCollection<IThreadChannel> Threads { get; internal set; }

    /// <summary>
    ///     Gets a thread member object for each returned thread the current user has joined.
    /// </summary>
    public IReadOnlyCollection<IThreadUser> ThreadMembers { get; internal set; }

    /// <summary>
    ///     Gets whether the index is not yet available for the guild. If true, the search results will be empty
    ///     and the client should retry after the delay specified in <see cref="RetryAfter"/>.
    /// </summary>
    public bool IndexNotYetAvailable { get; internal set; }

    /// <summary>
    ///     Gets the delay in seconds before the data should become available, if the index is not yet available.
    ///     This field is only present when <see cref="IndexNotYetAvailable"/> is true.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if the index is available.
    /// </remarks>
    public int? RetryAfter { get; internal set; }
}
