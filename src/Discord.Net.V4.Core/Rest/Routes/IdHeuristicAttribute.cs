namespace Discord;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
internal sealed class IdHeuristicAttribute<T> : Attribute
    where T : IEntity;
