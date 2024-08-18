namespace Discord;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property, AllowMultiple = true)]
internal sealed class IdHeuristicAttribute<T> : Attribute
    where T : IEntity;
