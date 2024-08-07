namespace Discord.Models;

public interface IInvalidSessionPayloadData : IGatewayPayloadData
{
    bool CanResume { get; }
}
