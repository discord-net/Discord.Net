using Discord.Models.Json;

namespace Discord.Rest;

public partial class Routes
{
    public static readonly IApiOutRoute<Models.Json.GetGatewayResponse> GetGateway =
        new ApiOutRoute<GetGatewayResponse>(
            nameof(GetGateway),
            RequestMethod.Get,
            "gateway"
        );

    public static readonly IApiOutRoute<Models.Json.GetGatewayBotResponse> GetGatewayBot = new ApiOutRoute<GetGatewayBotResponse>(
        nameof(GetGatewayBot),
        RequestMethod.Get,
        "gateway/bot"
    );
}
