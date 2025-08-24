using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IPollAnswerModel : IModel
{
    int AnswerId { get; }
    
    IPollMediaModel PollMedia { get; }
}