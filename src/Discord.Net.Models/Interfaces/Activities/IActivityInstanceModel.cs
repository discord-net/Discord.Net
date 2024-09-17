namespace Discord.Models;

[ModelEquality]
public partial interface IActivityInstanceModel : IModel
{
    string Id { get; }
}