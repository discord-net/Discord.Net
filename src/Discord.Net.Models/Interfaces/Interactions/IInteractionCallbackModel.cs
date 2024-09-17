namespace Discord.Models;

[ModelEquality]
public partial interface IInteractionCallbackModel : IEntityModel<ulong>
{
    int Type { get; }
    string? ActivityInstanceId { get; }
    ulong? ResponseMessageId { get; }
    bool? ResponseMessageLoading { get; }
    bool? ResponseMessageEphemeral { get; }
}