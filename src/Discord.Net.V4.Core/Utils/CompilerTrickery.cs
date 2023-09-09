// Allows for `required` keyword in net6
#if NET6_0
namespace System.Runtime.CompilerServices
{
    public class RequiredMemberAttribute : Attribute { }
    public class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string name) { }
    }
}
#endif
