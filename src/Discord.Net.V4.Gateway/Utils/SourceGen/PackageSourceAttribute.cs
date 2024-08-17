namespace Discord.Gateway;

#pragma warning disable CS9113 // Parameter is unread.

[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class PackageSourceAttribute(string source) : Attribute;

#pragma warning restore CS9113 // Parameter is unread.
