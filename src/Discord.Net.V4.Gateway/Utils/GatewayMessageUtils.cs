namespace Discord.Gateway;

public static class GatewayMessageUtils
{
    public static T AsGatewayPayloadData<T>(IGatewayMessage message, GatewayOpCode code)
        where T : IGatewayPayloadData
    {
        if(message.OpCode != code)
            throw new UnexpectedGatewayMessageException(code, message.OpCode);

        if (message.Payload is not T tPayload)
            throw new UnexpectedGatewayPayloadException(typeof(T), message.Payload);

        return tPayload;
    }
}
