using Discord.Models.Json;
using Discord.Utils;

namespace Discord.Rest;

public partial class Routes
{
    public static IApiInOutRoute<InteractionResponse, InteractionCallbackResponse> CreateInteractionResponse(
        [IdHeuristic<IInteraction>] ulong interactionId,
        [IdHeuristic<ITokenPathProvider>] string interactionToken,
        InteractionResponse body,
        bool? withResponse = null
    ) => new ApiInOutRoute<InteractionResponse, InteractionCallbackResponse>(
        nameof(CreateInteractionResponse),
        RequestMethod.Post,
        $"interactions/{interactionId}/{interactionToken}/callback{RouteUtils.GetUrlEncodedQueryParams(("with_response", withResponse))}",
        body
    );

    public static IApiOutRoute<Message> GetOriginalInteractionResponse(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<ITokenPathProvider>] string interactionToken
    ) => new ApiOutRoute<Message>(
        nameof(GetOriginalInteractionResponse),
        RequestMethod.Get,
        $"webhooks/{applicationId}/{interactionToken}/messages/@original"
    );

    public static IApiInOutRoute<ModifyWebhookMessageParams, Message> ModifyOriginalInteractionResponse(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<ITokenPathProvider>] string interactionToken,
        ModifyWebhookMessageParams body
    ) => new ApiInOutRoute<ModifyWebhookMessageParams, Message>(
        nameof(ModifyOriginalInteractionResponse),
        RequestMethod.Patch,
        $"webhooks/{applicationId}/{interactionToken}/messages/@original",
        body
    );

    public static IApiRoute DeleteOriginalInteractionResponse(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<ITokenPathProvider>] string interactionToken
    ) => new ApiRoute(
        nameof(DeleteOriginalInteractionResponse),
        RequestMethod.Delete,
        $"webhooks/{applicationId}/{interactionToken}/messages/@original"
    );

    public static IApiInOutRoute<ExecuteWebhookParams, Message> CreateFollowupMessage(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<ITokenPathProvider>] string interactionToken,
        ExecuteWebhookParams body
    ) => new ApiInOutRoute<ExecuteWebhookParams, Message>(
        nameof(CreateFollowupMessage),
        RequestMethod.Post,
        $"webhooks/{applicationId}/{interactionToken}",
        body
    );

    public static IApiOutRoute<Message> GetFollowupMessage(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<ITokenPathProvider>] string interactionToken,
        [IdHeuristic<IMessage>] ulong messageId
    ) => new ApiOutRoute<Message>(
        nameof(GetFollowupMessage),
        RequestMethod.Get,
        $"webhooks/{applicationId}/{interactionToken}/messages/{messageId}"
    );

    public static IApiInOutRoute<ModifyWebhookMessageParams, Message> ModifyFollowupMessage(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<ITokenPathProvider>] string interactionToken,
        [IdHeuristic<IMessage>] ulong messageId,
        ModifyWebhookMessageParams body
    ) => new ApiInOutRoute<ModifyWebhookMessageParams, Message>(
        nameof(ModifyFollowupMessage),
        RequestMethod.Patch,
        $"webhooks/{applicationId}/{interactionToken}/messages/{messageId}",
        body
    );

    public static IApiRoute DeleteFollowupMessage(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<ITokenPathProvider>] string interactionToken,
        [IdHeuristic<IMessage>] ulong messageId
    ) => new ApiRoute(
        nameof(DeleteFollowupMessage),
        RequestMethod.Delete,
        $"webhooks/{applicationId}/{interactionToken}/messages/{messageId}"
    );
}