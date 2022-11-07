using System.Collections.Generic;

namespace Discord;

public class WelcomeScreen
{
    /// <summary>
    ///     Gets the server description shown in the welcome screen. Null if not set.
    /// </summary>
    public string Description { get; }

    /// <summary>
    ///     Gets the channels shown in the welcome screen, up to 5 channels.
    /// </summary>
    public IReadOnlyCollection<WelcomeScreenChannel> Channels { get; }

    internal WelcomeScreen(string description, IReadOnlyCollection<WelcomeScreenChannel> channels)
    {
        Description = description;

        Channels = channels;
    }

}
