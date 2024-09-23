namespace Discord.Models;

[ModelEquality]
public partial interface IPollAnswerCountModel : IModel
{
    int Id { get; }
    int Count { get; }
    bool MeVoted { get; }
}