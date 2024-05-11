using System;

namespace Discord
{
    /// <summary>
    ///     Provides a series of helper methods for permissions.
    /// </summary>
    public static class PermissionUtils
    {
        /// <summary>
        ///     Determines the hierarchy of a target object based on its type.
        /// </summary>
        /// <param name="target">
        ///     The target object: <see cref="IRole"/>, <see cref="IGuildUser"/>, or <see cref="IUser"/>.
        /// </param>
        /// <returns>
        ///     An integer representing the hierarchy of the target.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when the parameter type is not supported by this precondition attribute.
        /// </exception>
        public static int GetHieararchy(object target) => target switch
        {
            // The order of cases here is important to determine the correct hierarchy value.
            IRole role => role.Position,
            IGuildUser guildUser => guildUser.Hierarchy,
            IUser => int.MinValue,
            _ => throw new ArgumentOutOfRangeException(nameof(target), "Cannot determine hierarchy for the provided target.")
        };
    }
}
