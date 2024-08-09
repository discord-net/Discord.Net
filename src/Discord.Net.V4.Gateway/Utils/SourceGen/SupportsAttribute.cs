using Discord.Gateway;

namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class SupportsAttribute(EventParameterDegree degree) : Attribute;
#pragma warning restore CS9113 // Parameter is unread.
