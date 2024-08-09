namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.Class)]
public class DispatchEventAttribute(string eventName) : Attribute;
#pragma warning restore CS9113 // Parameter is unread.
