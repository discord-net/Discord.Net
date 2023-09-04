namespace Discord;

internal readonly struct APIRoute
{
    public readonly string Name;
    public readonly string Route;
    public readonly ulong? Scope;
    public readonly ScopeType ScopeType;
}

internal readonly struct APIRoute<TBody>
{
    public readonly string Name;
    public readonly string Route;
    public readonly ulong? Scope;
    public readonly ScopeType ScopeType;
    public readonly TBody Body;
}
