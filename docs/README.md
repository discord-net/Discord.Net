# Instructions for Building Documentation

The documentation for the Discord.NET library uses [docfx][docfx-main]. [Instructions for installing this tool can be found here.][docfx-installing]

1. Navigate to the root of the repository.
2. (Optional) If you intend to target a specific version, ensure that you
have the correct version checked out.
3. Build the library. Run `dotnet build` in the root of this repository.
 Ensure that the build passes without errors.
4. Build the docs using `docfx .\docs\docfx.json`. Add the `--serve` parameter
to preview the site locally. Some elements of the page may appear incorrect
when not hosted by a server.
  - Remarks: According to the docfx website, this tool does work on Linux under mono.

[docfx-main]: https://dotnet.github.io/docfx/
[docfx-installing]: https://dotnet.github.io/docfx/tutorial/docfx_getting_started.html
