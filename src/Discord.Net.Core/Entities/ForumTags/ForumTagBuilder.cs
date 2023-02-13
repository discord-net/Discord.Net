#nullable enable
using System;

namespace Discord;

public class ForumTagBuilder
{
    private string? _name;
    private IEmote? _emoji;
    private bool _moderated;
    private ulong? _id;

    /// <summary>
    ///     Returns the maximum length of name allowed by Discord.
    /// </summary>
    public const int MaxNameLength = 20;

    /// <summary>
    ///     Gets or sets the snowflake Id of the tag.
    /// </summary>
    /// <remarks>
    ///     If set this will update existing tag or will create a new one otherwise.
    /// </remarks>
    public ulong? Id
    {
        get { return _id; }
        set { _id = value; }
    }

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
    /// <param name="id"> If set existing tag will be updated or a new one will be created otherwise.</param>
    /// <param name="name"> Name of the tag.</param>
    /// <param name="isModerated"> Sets whether this tag can only be added to or removed from threads by a member
    /// with the <see cref="GuildPermissions.ManageThreads"/> permission. </param>
    public ForumTagBuilder(string name, ulong? id = null, bool isModerated = false)
    {
        Name = name;
        IsModerated = isModerated;
        Id = id;
    }

    /// <summary>
    /// Initializes a new <see cref="ForumTagBuilder"/> class with values
    /// </summary>
    /// <param name="name"> Name of the tag.</param>
    /// <param name="id"> If set existing tag will be updated or a new one will be created otherwise.</param>
    /// <param name="emoji"> Display emoji of the tag.</param>
    /// <param name="isModerated"> Sets whether this tag can only be added to or removed from threads by a member
    /// with the <see cref="GuildPermissions.ManageThreads"/> permission. </param>
    public ForumTagBuilder(string name, ulong? id = null, bool isModerated = false, IEmote? emoji = null)
    {
        Name = name;
        Emoji = emoji;
        IsModerated = isModerated;
        Id = id;
    }

    /// <summary>
    /// Initializes a new <see cref="ForumTagBuilder"/> class with values
    /// </summary>
    /// /// <param name="name"> Name of the tag.</param>
    /// <param name="id"> If set existing tag will be updated or a new one will be created otherwise.</param>
    /// <param name="emoteId"> The id of custom Display emoji of the tag.</param>
    /// <param name="isModerated"> Sets whether this tag can only be added to or removed from threads by a member
    /// with the <see cref="GuildPermissions.ManageThreads"/> permission </param>
    public ForumTagBuilder(string name, ulong? id = null, bool isModerated = false, ulong? emoteId = null)
    {
        Name = name;
        if (emoteId is not null)
            Emoji = new Emote(emoteId.Value, null, false);
        IsModerated = isModerated;
        Id = id;
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
    /// Sets the id of the tag.
    /// </summary>
    /// <param name="id"> If set existing tag will be updated or a new one will be created otherwise.</param>
    /// <exception cref="ArgumentException">Name length must be less than or equal to <see cref="MaxNameLength"/>.</exception>
    public ForumTagBuilder WithId(ulong? id)
    {
        Id = id;
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
    /// Sets whether this tag can only be added to or removed from threads by a member
    /// with the <see cref="GuildPermissions.ManageThreads"/> permission
    /// </summary>
    public ForumTagBuilder WithModerated(bool moderated)
    {
        IsModerated = moderated;
        return this;
    }

    public override int GetHashCode() => base.GetHashCode();

    public override bool Equals(object? obj)
        => obj is ForumTagBuilder builder && Equals(builder);

    /// <summary>
    /// Gets whether supplied tag builder is equals to the current one.
    /// </summary>
    public bool Equals(ForumTagBuilder? builder)
        => builder is not null &&
           Id == builder.Id &&
           Name == builder.Name &&
           (Emoji is Emoji emoji && builder.Emoji is Emoji otherEmoji && emoji.Equals(otherEmoji) ||
            Emoji is Emote emote && builder.Emoji is Emote otherEmote && emote.Equals(otherEmote)) &&
           IsModerated == builder.IsModerated;

    public static bool operator ==(ForumTagBuilder? left, ForumTagBuilder? right)
    => left?.Equals(right) ?? right is null;

    public static bool operator !=(ForumTagBuilder? left, ForumTagBuilder? right) => !(left == right);
}
