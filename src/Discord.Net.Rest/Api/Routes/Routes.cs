using Discord.Models;

namespace Discord.Rest.Api;

public static partial class Routes
{
    public interface Expand<out T1, out T2>;

    public interface In<in TBody>;

    public interface Out<out TResult>;

    partial record UpdateMyUser :
        In<IModifyCurrentUserParams>,
        Out<ICurrentUserModel>;

    partial record GetUser :Out<IUserModel>;

    partial record GetMyUser : Out<ICurrentUserModel>;
}