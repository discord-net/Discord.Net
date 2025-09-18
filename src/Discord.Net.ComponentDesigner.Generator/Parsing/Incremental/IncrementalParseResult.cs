using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

namespace Discord.ComponentDesignerGenerator.Parser;

public readonly record struct IncrementalParseResult(
    IReadOnlyList<ICXNode> ReusedNodes,
    IReadOnlyList<ICXNode> NewNodes,
    IReadOnlyList<TextChange> Changes,
    TextChangeRange AppliedRange
);
