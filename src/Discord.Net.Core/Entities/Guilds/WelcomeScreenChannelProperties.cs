using System;
using System.Xml.Linq;

namespace Discord;

public class WelcomeScreenChannelProperties : ISnowflakeEntity
{
    /// <summary>
    /// 	Gets or sets the channel's id.
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// 	Gets or sets the description shown for the channel.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets the emoji for this channel. <see cref="Emoji"/> if it is unicode emoji, <see cref="Emote"/> if it is a custom one and <see langword="null"/> if none is set.
    /// </summary>
    /// <remarks>
    ///     If the emoji is <see cref="Emote"/> only the <see cref="Emote.Id"/> will be populated.
    ///     Use <see cref="IGuild.GetEmoteAsync"/> to get the emoji.
    /// </remarks>
    public IEmote Emoji { get; set; }

    /// <inheritdoc/>
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <summary>
    ///     Initializes a new instance of <see cref="WelcomeScreenChannelProperties"/>.
    /// </summary>
    /// <param name="id">Id if a channel.</param>
    /// <param name="description">Description for the channel in the welcome screen.</param>
    /// <param name="emoji">The emoji for the channel in the welcome screen.</param>
    public WelcomeScreenChannelProperties(ulong id, string description, IEmote emoji = null)
    {
        Id = id;
        Description = description;
        Emoji = emoji;
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="WelcomeScreenChannelProperties"/>.
    /// </summary>
    public WelcomeScreenChannelProperties() { }
    /// <summary>
    ///     Initializes a new instance of <see cref="WelcomeScreenChannelProperties"/>.
    /// </summary>
    /// <param name="channel">A welcome screen channel to modify.</param>
    /// <returns>A new instance of <see cref="WelcomeScreenChannelProperties"/>.</returns>
    public static WelcomeScreenChannelProperties FromWelcomeScreenChannel(WelcomeScreenChannel channel)
        => new(channel.Id, channel.Description, channel.Emoji);
}
