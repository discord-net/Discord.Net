using Discord.Models;

namespace Discord.Gateway;

public interface IDispatchPackage<TPayload> : IDispatchPackage where TPayload : class, IGatewayPayloadData;
public interface IDispatchPackage;
