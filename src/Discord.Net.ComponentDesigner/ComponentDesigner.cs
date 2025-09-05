using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

public static class ComponentDesigner
{
    // ReSharper disable once InconsistentNaming
    public static IMessageComponentBuilder cx(
        [StringSyntax("html")] DesignerInterpolationHandler designer
    ) => cx<IMessageComponentBuilder>(designer);

    // ReSharper disable once InconsistentNaming
    public static T cx<T>(
        [StringSyntax("html")] DesignerInterpolationHandler designer
    ) where T : IMessageComponentBuilder
        => throw new UnreachableException();
}
