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
    public bool IsAnimated { get; }

    public DiscordEmojiId(string emoji)
    {
        Name = emoji;
        Id = null;
    }

    public DiscordEmojiId(ulong snowflake, string name, bool isAnimated = false)
    {
        Id = snowflake;
        Name = name;
        IsAnimated = isAnimated;
    }

    internal DiscordEmojiId(string? name, ulong? snowflake, bool? isAnimated)
    {
        Name = name;
        Id = snowflake;
        IsAnimated = isAnimated ?? false;
    }

    public static bool TryParse(string value, out DiscordEmojiId result)
    {
        if (ulong.TryParse(value, out var id))
        {
            result = new(null, id, false);
            return true;
        }

        if (value.Contains(':'))
        {
            var split = value.Split(":", StringSplitOptions.RemoveEmptyEntries);

            if (split.Length is < 2 or > 3)
            {
                result = default;
                return false;
            }

            var idStr = split[^1];
            var name = split[^2];
            
            var isAnimated = split is ["a", _, _];

            if (!ulong.TryParse(idStr, out id))
            {
                result = default;
                return false;
            }
            
            result = new DiscordEmojiId(name, id, isAnimated);
            return true;
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
            (not null, not null) => IsAnimated ? $"a:{Name}:{Id.Value}" :  $":{Name}:{Id.Value}",
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
            return new PartialCustomEmote()
            {
                Name = Optional.FromNullable<string?>(Name),
                Id = Id.Value,
                Animated = IsAnimated
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
        => new(null, snowflake, false);
    
    public static implicit operator DiscordEmojiId((string Name, ulong Id) identifier)
        => new(identifier.Id, identifier.Name);
    
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