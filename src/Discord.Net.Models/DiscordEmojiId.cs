using System.Text.RegularExpressions;
using System.Web;
using Discord.Models;
using Discord.Models.Json;

namespace Discord;

public readonly struct DiscordEmojiId :
    IEquatable<DiscordEmojiId>
{
    public string? Name { get; }
    public ulong? Id { get; }

    public DiscordEmojiId(string emoji)
    {
        Name = emoji;
        Id = null;
    }

    public DiscordEmojiId(string name, ulong snowflake)
    {
        Name = name;
        Id = snowflake;
    }

    public DiscordEmojiId(ulong snowflake)
    {
        Id = snowflake;
        Name = null;
    }

    internal DiscordEmojiId(string? name, ulong? snowflake)
    {
        Name = name;
        Id = snowflake;
    }

    public static bool TryParse(string value, out DiscordEmojiId result)
    {
        if (ulong.TryParse(value, out var id))
        {
            result = new(id);
            return true;
        }

        if (value.Contains(':'))
        {
            var split = value.Split(':');

            if (split.Length != 2)
            {
                result = default;
                return false;
            }

            if (!ulong.TryParse(split[1], out id))
            {
                result = default;
                return false;
            }

            result = new DiscordEmojiId(split[0], id);
        }

        bool hasEmpty = false;
        foreach (Match match in EmojiRegex.Matches(value))
        {
            if (hasEmpty && match.Value == string.Empty)
            {
                // two empties in a row, a character that wasn't an emoji was found
                result = default;
                return false;
            }
            else if (match.Value == string.Empty)
                hasEmpty = true;
        }

        result = new DiscordEmojiId(value);
        return true;
    }

    public override string ToString()
    {
        return (Name, Id) switch
        {
            (not null, not null) => $"{Name}:{Id.Value}",
            (not null, null) => Name,
            (null, not null) => Id.Value.ToString(),
            _ => throw new ArgumentException("Neither 'name' or 'snowflake' was specified.")
        };
    }

    public string ToURLEncoded()
        => HttpUtility.UrlEncode(ToString());

    public IModel ToModel()
    {
        if (Id.HasValue)
            return new PartialGuildEmote()
            {
                Name = Optional.FromNullable<string?>(Name),
                Id = Id.Value
            };

        if (Name is null)
            throw new NullReferenceException("Both 'Name' and 'Id' cannot be null");
        
        return new Emoji()
        {
            Name = Name
        };
    }

    public bool Equals(DiscordEmojiId other)
    {
        return
            Name == other.Name &&
            Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is DiscordEmojiId other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((Name?.GetHashCode() ?? 0) * 397) ^ Id.GetHashCode();
        }
    }

    public static bool operator ==(DiscordEmojiId left, DiscordEmojiId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DiscordEmojiId left, DiscordEmojiId right)
    {
        return !left.Equals(right);
    }

    public static implicit operator DiscordEmojiId(string str)
        => TryParse(str, out var id)
            ? id
            : throw new ArgumentException($"The provided string '{str}' is not a valid emoji identifier");

    public static implicit operator DiscordEmojiId(ulong snowflake)
        => new(snowflake);
    
    public static implicit operator DiscordEmojiId((string Name, ulong Id) identifier)
        => new(identifier.Name, identifier.Id);
    
    // https://www.unicode.org/reports/tr51/#EBNF_and_Regex
    private static readonly Regex EmojiRegex = new(
        """
        \p{RI} \p{RI} 
        | \p{Emoji} 
          ( \p{EMod} 
          | \\x{FE0F} \\x{20E3}? 
          | [\\x{E0020}-\\x{E007E}]+ \\x{E007F}
          )?
          (\\x{200D}
            ( \p{RI} \p{RI}
            | \p{Emoji}
              ( \p{EMod} 
              | \\x{FE0F} \\x{20E3}? 
              | [\\x{E0020}-\\x{E007E}]+ \\x{E007F}
              )?
            )
          )*
        """);
}