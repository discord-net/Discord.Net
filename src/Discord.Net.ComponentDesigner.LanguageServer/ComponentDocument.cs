using Discord.CX.Parser;
using Microsoft.CodeAnalysis.Text;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Discord.ComponentDesigner.LanguageServer.CX;

public sealed class ComponentDocument
{
    private static readonly Dictionary<DocumentUri, ComponentDocument> _documents = [];

    public DocumentUri Uri { get; }

    public int? Version { get; }

    public CXDoc CX => _cxDoc ??= Parse();

    private string _source;

    private TextSpan? _incrementalChangeRange;

    private CXDoc? _cxDoc;

    public ComponentDocument(
        DocumentUri uri,
        string source,
        int? version
    )
    {
        Uri = uri;
        Version = version;
        _source = source;
    }

    private CXDoc Parse()
    {

    }

    public static ComponentDocument Create(
        DocumentUri uri,
        string content,
        int? version,
        CancellationToken token
    ) => _documents[uri] = new(uri, content, version);

    public void Update(
        int? version,
        Container<TextDocumentContentChangeEvent> changes,
        CancellationToken token
    )
    {
        if (Version.HasValue && Version == version) return;

        // build up the new source
        var sb = new StringBuilder(_source);
        var changeSpans = new List<TextSpan>();

        foreach (var change in changes)
        {
            if(change.Range is null) continue;

            if(change.Range.IsEmpty())
        }
    }

    public void Close()
    {
        _documents.Remove(Uri);
    }

    public static bool TryGet(DocumentUri uri, [MaybeNullWhen(false)] out ComponentDocument document)
        => _documents.TryGetValue(uri, out document);
}
