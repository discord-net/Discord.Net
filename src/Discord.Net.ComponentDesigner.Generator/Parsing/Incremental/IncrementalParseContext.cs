using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

namespace Discord.ComponentDesignerGenerator.Parser;

public readonly record struct IncrementalParseContext(
    IReadOnlyList<TextChange> Changes,
    TextChangeRange AffectedRange
);
