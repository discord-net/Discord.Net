namespace Discord.Models;

[ModelEquality]
public partial interface IInteractionCallbackResponseModel : IEntityModel<ulong>
{
    IInteractionCallbackModel Interaction { get; }
    IInteractionCallbackResourceModel? Resource { get; }

    ulong IEntityModel<ulong>.Id => Interaction.Id;
}