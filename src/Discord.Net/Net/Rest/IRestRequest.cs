using System.Collections.Generic;
using System.IO;

namespace Discord.Net.Rest
{
    public interface IRestRequest
    {
        string Method { get; }
        string Endpoint { get; }
        object Payload { get; }
    }
    public interface IRestRequest<TResponse> : IRestRequest
        where TResponse : class
    {
    }

    public interface IRestFileRequest : IRestRequest
    {
        string Filename { get; }
        Stream Stream { get; }
        IReadOnlyList<RestParameter> MultipartParameters { get; }
    }
    public interface IRestFileRequest<TResponse> : IRestFileRequest, IRestRequest<Message>
        where TResponse : class
    {
    }
}
