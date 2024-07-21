namespace Discord;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class VariableFuncArgsAttribute : Attribute
{
    public int InsertAt { get; set; }
}
