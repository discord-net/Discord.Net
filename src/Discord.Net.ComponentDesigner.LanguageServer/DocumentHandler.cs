using Discord.ComponentDesigner.LanguageServer.CX;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Discord.ComponentDesigner.LanguageServer;

public class DocumentHandler : TextDocumentSyncHandlerBase
{
    private readonly ILogger<DocumentHandler> _logger;

    private readonly TextDocumentSelector _documentSelector = new(
        new TextDocumentFilter {Pattern = "**/*.cx"}
    );

    public DocumentHandler(ILogger<DocumentHandler> logger)
    {
        _logger = logger;
    }

    public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri) =>
        throw new NotImplementedException();

    public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Opening {}", request.TextDocument.Uri);

        ComponentDocument.Create(
            request.TextDocument.Uri,
            request.TextDocument.Text,
            request.TextDocument.Version,
            cancellationToken
        );

        return Unit.Task;
    }

    public override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
    {
        if (!ComponentDocument.TryGet(request.TextDocument.Uri, out var document))
        {
            _logger.LogWarning("Unknown document update {}", request.TextDocument.Uri);
            return Unit.Task;
        }

        _logger.LogInformation("Updating {}", request.TextDocument.Uri);

        document.Update(request.TextDocument.Version, request.ContentChanges, cancellationToken);

        return Unit.Task;
    }

    public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        => Unit.Task;

    public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
    {
        if (!ComponentDocument.TryGet(request.TextDocument.Uri, out var document)) return Unit.Task;

        document.Close();
        return Unit.Task;
    }

    protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(
        TextSynchronizationCapability capability,
        ClientCapabilities clientCapabilities
    ) => new TextDocumentSyncRegistrationOptions()
    {
        Change = TextDocumentSyncKind.Incremental, Save = false, DocumentSelector = _documentSelector
    };
}
