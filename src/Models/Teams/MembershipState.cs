namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the membership state for a <see cref="TeamMember"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/topics/teams#data-models-membership-state-enum"/>
    /// </remarks>
    public enum MembershipState
    {
        /// <summary>
        /// This <see cref="TeamMember"/> has been invited, but did not accept yet.
        /// </summary>
        Invited = 1,

        /// <summary>
        /// This <see cref="TeamMember"/> accepted the invite.
        /// </summary>
        Accepted = 2,
    }
}
