using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

namespace Discord.CX.Parser;

public readonly record struct IncrementalParseContext(
    IReadOnlyList<TextChange> Changes,
    TextChangeRange AffectedRange
);
