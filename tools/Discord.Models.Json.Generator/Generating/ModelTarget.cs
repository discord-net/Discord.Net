using System.Reflection;
using Discord.Models.Json.Generator.Specs;

namespace Discord.Models.Json.Generator;

public sealed class ModelTarget
{
    public Type Type { get; }
    public TypeSpec TypeSpec { get; }
    
    public TypeSpec JsonContextPartialSpec { get; }
    public SpecModel SpecModel { get; }
    public IReadOnlyCollection<Type> Hierarchy { get; }
    
    public ModelTarget(
        Type type,
        TypeSpec typeSpec,
        SpecModel specModel
    )
    {
        Type = type;
        TypeSpec = typeSpec;
        SpecModel = specModel;
        Hierarchy = BFSHierarchy(type);

        JsonContextPartialSpec = new TypeSpec(
            "DiscordJsonContext",
            "class",
            modifiers: ["partial"]
        );
    }

    private static Type[] BFSHierarchy(Type type)
    {
        var result = new List<Type>();
        var seen = new HashSet<Type>();
        var queue = new Queue<Type>([type]);

        while (queue.TryDequeue(out var current))
        {
            if(!seen.Add(current)) continue;
            
            result.Add(current);
            foreach (var iface in current.GetInterfaces())
            {
                queue.Enqueue(iface);
            }
        }

        return result.ToArray();
    }

    public PropertyInfo? GetProperty(string name)
        => Hierarchy.Select(x => x.GetProperty(name)).FirstOrDefault(x => x is not null);
}