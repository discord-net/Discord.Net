namespace Discord.Models;

public interface IMentionedUser : IUserModel
{
    IPartialMemberModel Member { get; }
}
