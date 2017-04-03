# Contributing to Docs

I don't really have any strict conditions for writing documentation, 
but just keep these few guidelines in mind:

* Keep code samples in the `guides/samples` folder
* When referencing an object in the API, link to it's page in the 
API documentation.
* Documentation should be written in clear and proper English*

\* If anyone is interested in translating documentation into other 
languages, please open an issue or contact me on 
Discord (`foxbot#0282`).

### Layout

Documentation should be written in a FAQ/Wiki style format.

Recommended reads:

* http://docs.microsoft.com
* http://flask.pocoo.org/docs/0.12/

Style consistencies:

* Use a ruler set at 70 characters
* Links should use long syntax

Example of long link syntax:

```
Please consult the [API Documentation] for more information.

[API Documentation]: xref:System.String
```

### Compiling

Documentation is compiled into a static site using [DocFx]. 
We currently use the most recent build off the dev branch.

After making changes, compile your changes into the static site with 
`docfx`. You can also view your changes live with `docfx --serve`.

[DocFx]: https://dotnet.github.io/docfx/