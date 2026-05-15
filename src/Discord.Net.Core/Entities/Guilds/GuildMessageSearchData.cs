using System;
using System.Collections.Generic;

namespace Discord;

public class GuildMessageSearchData
{
    /// <summary>
    /// Whether the guild is undergoing a deep historical indexing operation.
    /// </summary>
    public bool DoingDeepHistoricalIndex { get; set; }

    /// <summary>
    /// The number of documents that have been indexed during the current index operation, if any.
    /// </summary>
    public Optional<int> DocumentsIndexed { get; set; }

    /// <summary>
    /// The total number of results that match the query.
    /// </summary>
    public int TotalResults { get; set; }

    /// <summary>
    /// An array of messages that match the query
    /// </summary>
    public IReadOnlyCollection<IMessage> Messages { get; set; }

    /// <summary>
    /// The threads that contain the returned messages.
    /// </summary>
    public Optional<IReadOnlyCollection<IThreadChannel>> Threads => throw new NotImplementedException();

    /// <summary>
    /// A thread member object for each returned thread the current user has joined.
    /// </summary>
    public Optional<IReadOnlyCollection<IThreadUser>> ThreadMembers => throw new NotImplementedException();
}
