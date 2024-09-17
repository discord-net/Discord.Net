namespace Discord;

public interface ITokenPathProvider : 
    IPathIdProvider<string>
{
    string Token { get; }

    string IIdentifiable<string>.Id => Token;
}