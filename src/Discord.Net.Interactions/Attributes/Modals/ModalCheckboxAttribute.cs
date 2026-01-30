using System;

namespace Discord.Interactions;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class ModalCheckboxAttribute : ModalInputAttribute
{
    public override ComponentType ComponentType => ComponentType.Checkbox;

    public ModalCheckboxAttribute(string customId, int id = 0)
        : base(customId, id)
    {
    }
}
