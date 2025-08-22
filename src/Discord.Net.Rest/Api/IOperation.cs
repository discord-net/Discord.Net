namespace Discord.Models.Rest.Api;

public interface IOperation : IRoute
{
    static abstract string OperationId { get; }
    static abstract RequestMethod Method { get; }
    static abstract AuthenticationScheme AuthenticationScheme { get; }

    string Format();
}

public static class OperationExtensions
{
    extension<T>(T operation) where T : IOperation
    {
        public void Foo(){}
        public RequestMethod Method => T.Method;
    }
}