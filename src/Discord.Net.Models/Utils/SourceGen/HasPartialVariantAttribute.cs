namespace Discord;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class HasPartialVariantAttribute : Attribute;

[AttributeUsage(AttributeTargets.Property)]
public sealed class PartialIgnoreAttribute : Attribute;

[AttributeUsage(AttributeTargets.Property)]
public sealed class NullableInPartialAttribute : Attribute;