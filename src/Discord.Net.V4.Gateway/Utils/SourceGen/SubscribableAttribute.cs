namespace Discord.Gateway;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal sealed class SubscribableAttribute<T> : Attribute
    where T : Delegate
{
    public SubscribableAttribute(){}
    public SubscribableAttribute(string name) { }
}
