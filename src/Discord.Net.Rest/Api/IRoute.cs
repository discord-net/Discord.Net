namespace Discord.Rest.Api;

public interface IRoute
{
    static abstract string Path { get; }
    static abstract IReadOnlyList<Type> RouteParameterTypes { get; }
    IReadOnlyList<RouteParameters> RouteParameters { get; }
}