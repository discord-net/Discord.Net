using System;

namespace Discord.Interactions;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class ModalCheckboxGroupAttribute : ModalInputAttribute
{
    public override ComponentType ComponentType => ComponentType.CheckboxGroup;

    public int MinValues { get; set; } = 1;

    public int MaxValues { get; set; } = 1;

    public ModalCheckboxGroupAttribute(string customId, int id = 0, int minValues = 1, int maxValues = 1)
        : base(customId, id)
    {
        MinValues = minValues;
        MaxValues = maxValues;
    }
}
