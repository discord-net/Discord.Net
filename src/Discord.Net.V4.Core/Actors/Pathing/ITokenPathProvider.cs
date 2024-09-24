namespace Discord;

public interface ITokenPathProvider : 
    IPathIdProvider<string>
{
    string Token { get; }

    string IPathIdProvider<string>.Id => Token;
}