using Discord.Models;
using Discord.Rest.Extensions;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Rest;

public sealed partial class RestMessageInteractionMetadata(
    DiscordRestClient client,
    RestMessage message,
    IMessageInteractionMetadataModel model
) :
    RestEntity<ulong, IMessageInteractionMetadataModel>(client, model.Id),
    IMessageInteractionMetadata
{
    public InteractionType Type => (InteractionType) Model.Type;

    [SourceOfTruth] public RestUserActor User => Client.Users[Model.UserId];

    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> AuthorizingIntegrationOwners
        => Computed(nameof(AuthorizingIntegrationOwners), model =>
            model.AuthorizingIntegrationOwners.ToImmutableDictionary(
                x => (ApplicationIntegrationType) x.Key,
                x => x.Value)
        );

    [SourceOfTruth]
    public RestMessageActor? OriginalResponseMessage
        => Computed(nameof(OriginalResponseMessage), model => 
            model.OriginalResponseMessageId.HasValue
                ? message.Channel.Messages[model.OriginalResponseMessageId.Value]
                : null
        );

    [SourceOfTruth]
    public RestMessageActor? InteractedMessage
        => Computed(nameof(InteractedMessage), model => 
            model.InteractedMessageId.HasValue
                ? message.Channel.Messages[model.InteractedMessageId.Value]
                : null
        );

    [SourceOfTruth]
    public RestMessageInteractionMetadata? TriggeringInteractionMetadata
        => Computed(nameof(TriggeringInteractionMetadata), model => 
            model.TriggeringInteractionMetadata is not null
                ? new RestMessageInteractionMetadata(Client, message, model.TriggeringInteractionMetadata)
                : null
        );

    internal override IMessageInteractionMetadataModel Model => _model;

    private IMessageInteractionMetadataModel _model = model;

    public ValueTask UpdateAsync(IMessageInteractionMetadataModel model, CancellationToken token = default)
    {
        _model = model;

        return ValueTask.CompletedTask;
    }

    public IMessageInteractionMetadataModel GetModel() => Model;
}