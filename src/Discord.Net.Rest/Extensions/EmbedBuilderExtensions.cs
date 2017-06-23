+using System;
+
+namespace Discord
+{
+    public static class EmbedBuilderExtensions
+    {
+        public static EmbedBuilder WithUrl(this EmbedBuilder builder, string url)
+        => Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri) ? builder.WithUrl(uri) : builder;
+
+        public static EmbedBuilder WithImageUrl(this EmbedBuilder builder, string url)
+        => Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri) ? builder.WithImageUrl(uri) : builder;
+
+        public static EmbedBuilder WithThumbnailUrl(this EmbedBuilder builder, string url)
+        => Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri) ? builder.WithThumbnailUrl(uri) : builder;
+
+        public static EmbedAuthorBuilder WithUrl(this EmbedAuthorBuilder builder, string url)
+        => Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri) ? builder.WithUrl(uri) : builder;
+
+        public static EmbedAuthorBuilder WithIconUrl(this EmbedAuthorBuilder builder, string url)
+        => Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri) ? builder.WithIconUrl(uri) : builder;
+
+        public static EmbedFooterBuilder WithIconUrl(this EmbedFooterBuilder builder, string url)
+        => Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri) ? builder.WithIconUrl(uri) : builder;
+    }
+}
