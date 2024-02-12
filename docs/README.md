# Instructions for Building Documentation

The documentation for the Discord.Net library uses [DocFX][docfx-main].
Instructions for installing this tool can be found [here][docfx-installing].

> [!IMPORTANT]
> You must use DocFX version **2.71.1** for everything to work.

1. Navigate to the root of the repository.
2. Build the docs using `docfx docs/docfx.json`. Add the `--serve`
 parameter to preview the site locally.

Please note that if you intend to target a specific version, ensure
that you have the correct version checked out.

[docfx-main]: https://dotnet.github.io/docfx/
[docfx-installing]: https://dotnet.github.io/docfx/tutorial/docfx_getting_started.html
