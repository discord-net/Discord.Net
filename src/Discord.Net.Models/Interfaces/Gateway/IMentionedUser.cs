namespace Discord.Models;

public interface IMentionedUser : IUserModel
{
    IMemberModel Member { get; }
}
