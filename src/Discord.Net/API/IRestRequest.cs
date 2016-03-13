using System.IO;

namespace Discord.API
{
    public interface IRestRequest
    {
        string Method { get; }
        string Endpoint { get; }
        object Payload { get; }
    }
    public interface IRestRequest<ResponseT> : IRestRequest
        where ResponseT : class
    {
    }

    public interface IRestFileRequest : IRestRequest
    {
        string Filename { get; }
        Stream Stream { get; }
    }
    public interface IRestFileRequest<ResponseT> : IRestFileRequest, IRestRequest<Message>
        where ResponseT : class
    {
    }
}
