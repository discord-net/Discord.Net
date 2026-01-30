using System;

namespace Discord.Interactions;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class ModalRadioGroupAttribute : ModalInputAttribute
{
    public override ComponentType ComponentType => ComponentType.RadioGroup;

    public ModalRadioGroupAttribute(string customId, int id = 0)
        : base(customId, id)
    {
    }
}
