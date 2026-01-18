namespace Discord.Interactions;

/// <summary>
///     Marks a <see cref="IModal"/> property as a role select input.
/// </summary>
public class ModalRoleSelectAttribute : ModalSelectComponentAttribute
{
    /// <inheritdoc/>
    public override ComponentType ComponentType => ComponentType.RoleSelect;

    /// <summary>
    ///     Create a new <see cref="ModalRoleSelectAttribute"/>.
    /// </summary>
    /// <param name="customId">Custom ID of the role select component.</param>
    /// <param name="minValues">Minimum number of values that can be selected.</param>
    /// <param name="maxValues">Maximum number of values that can be selected.</param>
    /// <param name="id">The optional identifier for the component.</param>
    public ModalRoleSelectAttribute(string customId, int minValues = 1, int maxValues = 1, int? id = null)
        : base(customId, minValues, maxValues, id) { }
}
