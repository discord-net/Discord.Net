using System;

namespace Discord.Interactions
{
    internal interface ITypeHandler
    {
        public bool CanConvertTo(Type type);
    }
}
