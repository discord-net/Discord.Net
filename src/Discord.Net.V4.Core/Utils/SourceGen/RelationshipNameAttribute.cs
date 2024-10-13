namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.Interface)]
internal sealed class RelationshipNameAttribute(string name) : Attribute;
#pragma warning restore CS9113 // Parameter is unread.
