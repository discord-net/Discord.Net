using Discord.Net.Hanz.Tasks.Actors.Links.V4.SourceTypes;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;

public interface ITypeImplementerNode : ITypeProducerNode
{
    bool WillGenerateImplementation { get; }
    string ImplementationClassName { get; }
    Constructor? Constructor { get; }
    List<Property> Properties { get; }
    // List<(string Type, string Name, string? Default)> RequiredMembers { get; }
    // List<(string Type, string Name, string? Default)> ConstructorMembers { get; }
}

public static class TypeImplementerNodeExtensions
{
    // public static List<(string Type, string Name, string? Default)> GetParentsRequiredMembers<T>(
    //     this T node
    // ) where T : LinkNode, ITypeImplementerNode
    // {
    //     return node.Parents.OfType<ITypeImplementerNode>()
    //         .SelectMany(x => x.RequiredMembers)
    //         .ToList();
    // }
}