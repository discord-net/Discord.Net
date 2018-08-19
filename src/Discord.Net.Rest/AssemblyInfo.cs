using System.Runtime.CompilerServices;
using Discord;

[assembly: InternalsVisibleTo("Discord.Net.Rpc")]
[assembly: InternalsVisibleTo("Discord.Net.WebSocket")]
[assembly: InternalsVisibleTo("Discord.Net.Webhook")]
[assembly: InternalsVisibleTo("Discord.Net.Commands")]
[assembly: InternalsVisibleTo("Discord.Net.Tests")]

[assembly: TypeForwardedTo(typeof(Embed))]
[assembly: TypeForwardedTo(typeof(EmbedBuilder))]
[assembly: TypeForwardedTo(typeof(EmbedBuilderExtensions))]
[assembly: TypeForwardedTo(typeof(EmbedAuthor))]
[assembly: TypeForwardedTo(typeof(EmbedAuthorBuilder))]
[assembly: TypeForwardedTo(typeof(EmbedField))]
[assembly: TypeForwardedTo(typeof(EmbedFieldBuilder))]
[assembly: TypeForwardedTo(typeof(EmbedFooter))]
[assembly: TypeForwardedTo(typeof(EmbedFooterBuilder))]
[assembly: TypeForwardedTo(typeof(EmbedImage))]
[assembly: TypeForwardedTo(typeof(EmbedProvider))]
[assembly: TypeForwardedTo(typeof(EmbedThumbnail))]
[assembly: TypeForwardedTo(typeof(EmbedType))]
[assembly: TypeForwardedTo(typeof(EmbedVideo))]
