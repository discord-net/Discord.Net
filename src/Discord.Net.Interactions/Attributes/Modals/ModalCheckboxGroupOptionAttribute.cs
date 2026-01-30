using System;

namespace Discord.Interactions;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ModalCheckboxGroupOptionAttribute : Attribute
{
    public string Value { get; set; }

    public string Label { get; set; }

    public string Description { get; set; }

    public bool DefaultState { get; set; }

    public ModalCheckboxGroupOptionAttribute(string value, string label, string description = null, bool defaultState = false)
    {
        Value = value;
        Label = label;
        Description = description;
        DefaultState = defaultState;
    }
}
