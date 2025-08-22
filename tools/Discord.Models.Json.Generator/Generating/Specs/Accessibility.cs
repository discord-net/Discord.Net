namespace Discord.Models.Json.Generator.Specs;


public static class AccessibilityExt
{
    public static string ToKeywords(this Accessibility accessibility)
    {
        switch (accessibility)
        {
            case Accessibility.NotApplicable:
                return string.Empty;
            case Accessibility.Private:
                return "private";
            case Accessibility.ProtectedAndInternal:
                return "private protected";
            case Accessibility.Internal:
                return "internal";
            case Accessibility.Protected:
                return "protected";
            case Accessibility.ProtectedOrInternal:
                return "protected internal";
            case Accessibility.Public:
                return "public";
            default:
                throw new ArgumentException();
        }
    }
}

public enum Accessibility
{
    /// <summary>
    /// No accessibility specified.
    /// </summary>
    NotApplicable = 0,

    // DO NOT CHANGE ORDER OF THESE ENUM VALUES
    Private = 1,

    /// <summary>
    /// Only accessible where both protected and internal members are accessible
    /// (more restrictive than <see cref="Protected"/>, <see cref="Internal"/> and <see cref="ProtectedOrInternal"/>).
    /// </summary>
    ProtectedAndInternal = 2,

    /// <summary>
    /// Only accessible where both protected and friend members are accessible
    /// (more restrictive than <see cref="Protected"/>, <see cref="Friend"/> and <see cref="ProtectedOrFriend"/>).
    /// </summary>
    ProtectedAndFriend = ProtectedAndInternal,

    Protected = 3,

    Internal = 4,
    Friend = Internal,

    /// <summary>
    /// Accessible wherever either protected or internal members are accessible
    /// (less restrictive than <see cref="Protected"/>, <see cref="Internal"/> and <see cref="ProtectedAndInternal"/>).
    /// </summary>
    ProtectedOrInternal = 5,

    /// <summary>
    /// Accessible wherever either protected or friend members are accessible
    /// (less restrictive than <see cref="Protected"/>, <see cref="Friend"/> and <see cref="ProtectedAndFriend"/>).
    /// </summary>
    ProtectedOrFriend = ProtectedOrInternal,

    Public = 6
}