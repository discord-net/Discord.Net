namespace Discord.Models.Rest.Api;

public interface IRoute
{
    static abstract string Path { get; }
}