using System;

namespace Discord.Commands
{
    /// <summary>
    ///     When applied to an optional command parameter,
    ///     indicates that its default value should be used
    ///     if the TypeReader fails to read a valid value.
    /// </summary>
    /// <example>
    ///     <code language="cs">
    ///     [Command("stats")]
    ///     public async Task GetStatsAsync([FallbackToDefault] IUser user = null)
    ///     {
    ///         if (user is null)
    ///         {
    ///             await ReplyAsync("Couldn't find that user");
    ///             return;
    ///         }
    /// 
    ///         // ...pull stats
    ///     }
    ///     </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class FallbackToDefaultAttribute : Attribute { }
}
