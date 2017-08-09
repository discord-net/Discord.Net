using System;

namespace Discord.Serialization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ModelSelectorAttribute : Attribute
    {
        public string SelectorProperty { get; }
        public string SelectorGroup { get; }

        public ModelSelectorAttribute(string selectorProperty, string selectorGroup)
        {
            SelectorProperty = selectorProperty;
            SelectorGroup = selectorGroup;
        }
    }
}
