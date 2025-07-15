using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Utils.Bakery;

public static class Implementations
{
    public static TypeSpec ExplicitlyImplements(
        this TypeSpec source,
        TypeSpec type,
        MethodSpec method,
        MethodSpec target
    )
    {
        return source
            .AddMethods(
                target with
                {
                    ExplicitInterfaceImplementation = type.ToReferenceName(),
                    Parameters = method.Parameters,
                    ReturnType = method.ReturnType,
                    Accessibility = Accessibility.NotApplicable,
                    Modifiers = method.Modifiers.Union(target.Modifiers).Distinct().ToImmutableEquatableArray(),
                    Expression = target.ToInvocationString()
                }
            );
    }
}