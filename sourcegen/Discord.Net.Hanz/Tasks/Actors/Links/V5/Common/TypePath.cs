using System.Collections;
using System.Text;
using Discord.Net.Hanz.Utils.Bakery;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;

public interface IPathedState
{
    TypePath Path { get; }
}

public readonly record struct TypePath(
    ImmutableEquatableArray<TypePath.Part> Parts
) : IReadOnlyCollection<TypePath.Part>
{
    public static readonly TypePath Empty = new([]);

    public bool IsEmpty => Parts.Count == 0;
    public int Count => Parts.Count;

    public Part? Last => Parts.Count == 0 ? null : Parts[Parts.Count - 1];

    public readonly record struct Part(
        Type Type,
        string Name
    )
    {
        public override string ToString() => Name;

        public static implicit operator Part((Type, string) tuple) => new(tuple.Item1, tuple.Item2);
    }

    public IEnumerable<TypePath> CartesianProduct(bool removeLast = true)
        => Node.GetProduct(Parts, removeLast).Select(x => new TypePath(x.ToImmutableEquatableArray()));

    public bool Equals(params Type[] semantic)
    {
        if (Count != semantic.Length)
            return false;

        for (var i = 0; i < Parts.Count; i++)
        {
            if (Parts[i].Type != semantic[i])
                return false;
        }

        return true;
    }

    public bool StartsWith(params Type[] semantic)
        => SliceEquals(0, semantic);
    
    public bool EndsWith(params Type[] semantic)
        => SliceEquals(Count - semantic.Length, semantic);
    
    public bool SliceEquals(int start, params Type[] semantic)
    {
        if (start < 0 || start + semantic.Length > Count || semantic.Length == 0)
            return false;

        for (var i = 0; i < semantic.Length; i++)
        {
            if (Parts[start + i].Type != semantic[i])
                return false;
        }

        return true;
    }

    public TypePath Add<TPart>(string name)
        => new(Parts.Add(new(typeof(TPart), name)));

    public TypePath AddRange(IEnumerable<Part> parts)
        => new(Parts.AddRange(parts));

    public bool Contains<TPart>()
        => Parts.Any(x => x.Type == typeof(TPart));
    
    public int CountOfType<TPart>() => Parts.Count(x => x.Type == typeof(TPart));
    public int CountOfType(Type type) => Parts.Count(x => x.Type == type);

    public override string ToString()
        => Format();

    public string FormatRelative()
        => Format(exclude: [typeof(ActorNode)]);

    public string FormatParent()
        => Format(to: Count - 1);

    public TypePath OfType<T>()
        => new(new(Parts.Where(x => x.Type == typeof(T))));

    public TypePath Slice(int start = 0, int count = int.MaxValue)
    {
        if (start >= Count || count <= 0)
            return Empty;

        return new(new(Parts.Skip(start).Take(Math.Min(count, Parts.Count - start))));
    }

    public string Format(
        Type[]? include = null,
        Type[]? exclude = null,
        int from = 0,
        int to = int.MaxValue,
        bool prefixDot = false
    )
    {
        if (Parts.Count == 0) return string.Empty;

        var builder = new StringBuilder();

        if (prefixDot)
            builder.Append('.');

        bool hasFirst = false;

        for (var i = from; i < Math.Min(Parts.Count, to); i++)
        {
            var part = Parts[i];

            if (include is not null)
            {
                if (!include.Contains(part.Type)) continue;
            }
            else if (exclude is not null)
            {
                if (exclude.Contains(part.Type)) continue;
            }

            if (hasFirst)
                builder.Append('.');
            
            builder.Append(part);

            hasFirst = true;
        }

        return builder.ToString();
    }

    public static TypePath operator +(TypePath a, TypePath b)
        => new(a.Parts.AddRange(b.Parts));

    public static TypePath operator +(TypePath a, Part b)
        => new(a.Parts.Add(b));

    public static TypePath operator +(TypePath a, (Type, string) b)
        => new(a.Parts.Add(b));

    public static TypePath operator --(TypePath path)
        => path.Slice(count: path.Count - 1);
    
    public static implicit operator string(TypePath path) => path.ToString();
    
    public IEnumerator<Part> GetEnumerator()
    {
        return Parts.GetUnderlyingEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable) Parts).GetEnumerator();
    }
}