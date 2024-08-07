namespace Discord.Models;

public interface IResumePayloadData : IGatewayPayloadData
{
    string SessionToken { get; }
    string SessionId { get; }
    int Sequence { get; }
}
