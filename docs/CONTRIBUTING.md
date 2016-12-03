# Contributing to Docs

I don't really have any strict conditions for writing documentation, but just keep these few guidelines in mind:

* Keep code samples in the `guides/samples` folder
* When referencing an object in the API, link to it's page in the API documentation.
* Documentation should be written in clear and proper English*

\* If anyone is interested in translating documentation into other languages, please open an issue or contact me on Discord (`foxbot#0282`).

### Compiling

Documentation is compiled into a static site using [DocFx](https://dotnet.github.io/docfx/). We currently use version 2.8

After making changes, compile your changes into the static site with `docfx`. You can also view your changes live with `docfx --serve`.