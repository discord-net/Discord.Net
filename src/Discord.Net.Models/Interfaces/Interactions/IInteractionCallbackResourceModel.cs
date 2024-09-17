namespace Discord.Models;

[ModelEquality]
public partial interface IInteractionCallbackResourceModel : IModel
{
    int Type { get; }
    IActivityInstanceModel? ActivityInstance { get; }
    IMessageModel? Message { get; }
}