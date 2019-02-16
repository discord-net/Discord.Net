# Contributing to Docs

First of all, thank you for your interest in contributing to our
documentation work. We really appreciate it! That being said,
there are several guidelines you should attempt to follow when adding
to/changing the documentation.

## General Guidelines

* Keep code samples in each section's `samples` folder
* When referencing an object in the API, link to its page in the
 API documentation
* Documentation should be written in an FAQ/Wiki-style format
* Documentation should be written in clear and proper English*

\* If anyone is interested in translating documentation into other
languages, please open an issue or contact `foxbot#0282` on
Discord.

## XML Docstrings Guidelines

* When using the `<summary>` tag, use concise verbs. For example:

```cs
/// <summary> Gets or sets the guild user in this object. </summary>
public IGuildUser GuildUser { get; set; }
```

* The `<summary>` tag should not be more than 3 lines long. Consider
simplifying the terms or using the `<remarks>` tag instead.
* When using the `<code>` tag, put the code sample within the
`src/Discord.Net.Examples` project under the corresponding path of
the object and surround the code with a `#region` tag.
* If the remarks you are looking to write are too long, consider
writing a shorter version in the XML docs while keeping the longer
version in the `overwrites` folder using the DocFX overwrites syntax.
  * You may find an example of this in the samples provided within
  the folder.

## Docs Guide Guidelines

* Use a ruler set at 70 characters (use the docs workspace provided
if you are using Visual Studio Code)
* Links should use long syntax
* Pages should be short and concise, not broad and long

Example of long link syntax:

```md
Please consult the [API Documentation] for more information.

[API Documentation]: xref:System.String
```

## Recommended Reads

* [Microsoft Docs](https://docs.microsoft.com)
* [Flask Docs](https://flask.pocoo.org/docs/1.0/)
* [DocFX Manual](https://dotnet.github.io/docfx/)
* [Sandcastle XML Guide](http://ewsoftware.github.io/XMLCommentsGuide)