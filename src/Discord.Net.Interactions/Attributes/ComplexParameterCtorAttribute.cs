using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Tag a type constructor as the preferred Complex command constructor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    public class ComplexParameterCtorAttribute : Attribute { }
}
