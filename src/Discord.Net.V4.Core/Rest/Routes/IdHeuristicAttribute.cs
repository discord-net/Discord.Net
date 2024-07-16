namespace Discord;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Class | AttributeTargets.Struct)]
internal sealed class IdHeuristicAttribute<T> : Attribute
    where T : IEntity;
