#nullable enable
using System;

namespace Discord;

public class ForumTagBuilder
{
    private string? _name;
    private IEmote? _emoji;
    private bool _moderated;

    /// <summary>
    ///     Returns the maximum length of name allowed by Discord.
    /// </summary>
    public const int MaxNameLength = 20;

    /// <summary>
    ///     Gets or sets the name of the tag.
    /// </summary>
    /// <exception cref="ArgumentException">Name length must be less than or equal to <see cref="MaxNameLength"/>.</exception>
    public string? Name
    {
        get { return _name; }
        set
        {
            if (value?.Length > MaxNameLength)
                throw new ArgumentException(message: $"Name length must be less than or equal to {MaxNameLength}.", paramName: nameof(Name));
            _name = value;
        }
    }

    /// <summary>
    ///     Gets or sets the emoji of the tag.
    /// </summary>
    public IEmote? Emoji
    {
        get { return _emoji; }
        set { _emoji = value; }
    }

    /// <summary>
    /// Gets or sets whether this tag can only be added to or removed from threads by a member
    /// with the <see cref="GuildPermissions.ManageThreads"/> permission
    /// </summary>
    public bool IsModerated
    {
        get { return _moderated; }
        set { _moderated = value; }
    }

    /// <summary>
    /// Initializes a new <see cref="ForumTagBuilder"/> class.
    /// </summary>
    public ForumTagBuilder()
    {

    }

    /// <summary>
    /// Initializes a new <see cref="ForumTagBuilder"/> class with values
    /// </summary>
    public ForumTagBuilder(string name)
    {
        Name = name;
        IsModerated = false;
    }

    /// <summary>
    /// Initializes a new <see cref="ForumTagBuilder"/> class with values
    /// </summary>
    public ForumTagBuilder(string name, IEmote? emoji = null, bool moderated = false)
    {
        Name = name;
        Emoji = emoji;
        IsModerated = moderated;
    }

    /// <summary>
    /// Initializes a new <see cref="ForumTagBuilder"/> class with values
    /// </summary>
    public ForumTagBuilder(string name, ulong? emoteId = null, bool moderated = false)
    {
        Name = name;
        if(emoteId is not null)
            Emoji = new Emote(emoteId.Value, null,  false);
        IsModerated = moderated;
    }

    /// <summary>
    /// Builds the Tag.
    /// </summary>
    /// <returns>An instance of <see cref="ForumTagProperties"/></returns>
    /// <exception cref="ArgumentNullException">"Name must be set to build the tag"</exception>
    public ForumTagProperties Build()
    {
        if (_name is null)
            throw new ArgumentNullException(nameof(Name), "Name must be set to build the tag");
        return new ForumTagProperties(_name!, _emoji, _moderated);
    }

    /// <summary>
    /// Sets the name of the tag.
    /// </summary>
    /// <exception cref="ArgumentException">Name length must be less than or equal to <see cref="MaxNameLength"/>.</exception>
    public ForumTagBuilder WithName(string name)
    {
        Name = name;
        return this;
    }

    /// <summary>
    /// Sets the emoji of the tag.
    /// </summary>
    public ForumTagBuilder WithEmoji(IEmote? emoji)
    {
        Emoji = emoji;
        return this;
    }

    /// <summary>
    /// Sets the IsModerated of the tag.
    /// </summary>
    public ForumTagBuilder WithModerated(bool moderated)
    {
        IsModerated = moderated;
        return this;
    }
}
