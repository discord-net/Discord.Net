using System;

namespace Discord.Interactions;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ModalRadioGroupOptionAttribute : Attribute
{
    public string Value { get; set; }

    public string Label { get; set; }

    public string Description { get; set; }

    public bool IsDefault { get; set; }

    public ModalRadioGroupOptionAttribute(string value, string label, string description = null, bool isDefault = false)
    {
        Value = value;
        Label = label;
        Description = description;
        IsDefault = isDefault;
    }
}
