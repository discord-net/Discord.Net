namespace Discord;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class SubscribableAttribute<T> : Attribute
    where T : Delegate;
