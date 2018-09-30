using System;

namespace Discord.Commands
{
    /// <summary>
    ///     Prevents the marked property from being injected into a module.
    /// </summary>
    /// <remarks>
    ///     This attribute prevents the marked member from being injected into its parent module. Useful when you have a
    ///     public property that you do not wish to invoke the library's dependency injection service.
    /// </remarks>
    /// <example>
    ///     In the following example, <c>DatabaseService</c> will not be automatically injected into the module and will
    ///     not throw an error message if the dependency fails to be resolved.
    ///     <code language="cs">
    ///     public class MyModule : ModuleBase
    ///     {
    ///         [DontInject]
    ///         public DatabaseService DatabaseService;
    ///         public MyModule()
    ///         {
    ///             DatabaseService = DatabaseFactory.Generate();
    ///         }
    ///     }
    ///     </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DontInjectAttribute : Attribute
    {
    }
}
