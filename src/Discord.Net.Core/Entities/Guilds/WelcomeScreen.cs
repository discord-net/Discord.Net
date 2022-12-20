using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord;

public class WelcomeScreen
{
    /// <summary>
    ///     Gets the server description shown in the welcome screen. <see langword="null"/> if not set.
    /// </summary>
    public string Description { get; }

    /// <summary>
    ///     Gets the channels shown in the welcome screen, up to 5 channels.
    /// </summary>
    public IReadOnlyCollection<WelcomeScreenChannel> Channels { get; }

    internal WelcomeScreen(string description, IReadOnlyCollection<WelcomeScreenChannel> channels)
    {
        Description = description;

        Channels = channels.ToImmutableArray();
    }
}
