using System;
using System.Collections.Generic;
using Xunit;

namespace Discord
{
    /// <summary>
    ///     Tests the <see cref="Discord.EmbedBuilder"/> class.
    /// </summary>
    public class EmbedBuilderTests
    {
        private const string Name = "chrisj";
        private const string Icon = "https://meowpuffygottem.fun/blob.png";
        private const string Url = "https://meowpuffygottem.fun/";

        /// <summary>
        ///     Tests the behavior of <see cref="EmbedBuilder.WithAuthor(string, string, string)"/>.
        /// </summary>
        [Fact]
        public void WithAuthor_Strings()
        {
            var builder = new EmbedBuilder();
            // null by default
            Assert.Null(builder.Author);

            builder = new EmbedBuilder()
                .WithAuthor(Name, Icon, Url);

            Assert.NotNull(builder.Author);
            Assert.Equal(Name, builder.Author.Name);
            Assert.Equal(Icon, builder.Author.IconUrl);
            Assert.Equal(Url, builder.Author.Url);
        }

        /// <summary>
        ///     Tests the behavior of <see cref="EmbedBuilder.WithAuthor(EmbedAuthorBuilder)"/>
        /// </summary>
        [Fact]
        public void WithAuthor_AuthorBuilder()
        {
            var author = new EmbedAuthorBuilder()
                .WithIconUrl(Icon)
                .WithName(Name)
                .WithUrl(Url);
            var builder = new EmbedBuilder()
                .WithAuthor(author);
            Assert.NotNull(builder.Author);
            Assert.Equal(Name, builder.Author.Name);
            Assert.Equal(Icon, builder.Author.IconUrl);
            Assert.Equal(Url, builder.Author.Url);
        }

        /// <summary>
        ///     Tests the behavior of <see cref="EmbedBuilder.WithAuthor(Action{EmbedAuthorBuilder})"/>
        /// </summary>
        [Fact]
        public void WithAuthor_ActionAuthorBuilder()
        {
            var builder = new EmbedBuilder()
                .WithAuthor((author) =>
                author.WithIconUrl(Icon)
                .WithName(Name)
                .WithUrl(Url));
            Assert.NotNull(builder.Author);
            Assert.Equal(Name, builder.Author.Name);
            Assert.Equal(Icon, builder.Author.IconUrl);
            Assert.Equal(Url, builder.Author.Url);
        }

        /// <summary>
        ///     Tests the behavior of <see cref="EmbedAuthorBuilder"/>.
        /// </summary>
        [Fact]
        public void EmbedAuthorBuilder()
        {
            var builder = new EmbedAuthorBuilder()
                .WithIconUrl(Icon)
                .WithName(Name)
                .WithUrl(Url);
            Assert.Equal(Icon, builder.IconUrl);
            Assert.Equal(Name, builder.Name);
            Assert.Equal(Url, builder.Url);
        }

        /// <summary>
        ///     Tests that invalid titles throw an <see cref="ArgumentException"/>.
        /// </summary>
        /// <param name="title">The embed title to set.</param>
        [Theory]
        // 257 chars
        [InlineData("jVyLChmA7aBZozXQuZ3VDEcwW6zOq0nteOVYBZi31ny73rpXfSSBXR4Jw6FiplDKQseKskwRMuBZkUewrewqAbkBZpslHirvC5nEzRySoDIdTRnkVvTXZUXg75l3bQCjuuHxDd6DfrY8ihd6yZX1Y0XFeg239YBcYV4TpL9uQ8H3HFYxrWhLlG2PRVjUmiglP5iXkawszNwMVm1SZ5LZT4jkMZHxFegVi7170d16iaPWOovu50aDDHy087XBtLKVa")]
        // 257 chars of whitespace
        [InlineData("                                                                                                                                                                                                                                                                 ")]
        public void Title_Invalid(string title)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var builder = new EmbedBuilder
                {
                    Title = title
                };
            });
            Assert.Throws<ArgumentException>(() =>
            {
                new EmbedBuilder().WithTitle(title);
            });
        }

        /// <summary>
        ///     Tests that valid titles do not throw any exceptions.
        /// </summary>
        /// <param name="title">The embed title to set.</param>
        [Theory]
        // 256 chars
        [InlineData("jVyLChmA7aBZozXQuZ3VDEcwW6zOq0nteOVYBZi31ny73rpXfSSBXR4Jw6FiplDKQseKskwRMuBZkUewrewqAbkBZpslHirvC5nEzRySoDIdTRnkVvTXZUXg75l3bQCjuuHxDd6DfrY8ihd6yZX1Y0XFeg239YBcYV4TpL9uQ8H3HFYxrWhLlG2PRVjUmiglP5iXkawszNwMVm1SZ5LZT4jkMZHxFegVi7170d16iaPWOovu50aDDHy087XBtLKV")]
        public void Tile_Valid(string title)
        {
            var builder = new EmbedBuilder
            {
                Title = title
            };
            new EmbedBuilder().WithTitle(title);
        }

        /// <summary>
        ///     Tests that invalid descriptions throw an <see cref="ArgumentException"/>.
        /// </summary>
        [Fact]
        public void Description_Invalid()
        {
            IEnumerable<string> GetInvalid()
            {
                yield return new string('a', 4097);
            }
            foreach (var description in GetInvalid())
            {
                Assert.Throws<ArgumentException>(() => new EmbedBuilder().WithDescription(description));
                Assert.Throws<ArgumentException>(() =>
                {
                    var b = new EmbedBuilder
                    {
                        Description = description
                    };
                });
            }
        }

        /// <summary>
        ///     Tests that valid descriptions do not throw any exceptions.
        /// </summary>
        [Fact]
        public void Description_Valid()
        {
            IEnumerable<string> GetValid()
            {
                yield return string.Empty;
                yield return null;
                yield return new string('a', 4096);
            }
            foreach (var description in GetValid())
            {
                var b = new EmbedBuilder().WithDescription(description);
                Assert.Equal(description, b.Description);

                b = new EmbedBuilder
                {
                    Description = description
                };
                Assert.Equal(description, b.Description);
            }
        }

        /// <summary>
        ///     Tests that valid url does not throw any exceptions.
        /// </summary>
        /// <param name="url">The url to set.</param>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("https://docs.stillu.cc")]
        public void Url_Valid(string url)
        {
            // does not throw an exception
            var result = new EmbedBuilder()
                .WithUrl(url)
                .WithImageUrl(url)
                .WithThumbnailUrl(url);
            Assert.Equal(result.Url, url);
            Assert.Equal(result.ImageUrl, url);
            Assert.Equal(result.ThumbnailUrl, url);

            result = new EmbedBuilder
            {
                Url = url,
                ImageUrl = url,
                ThumbnailUrl = url
            };
            Assert.Equal(result.Url, url);
            Assert.Equal(result.ImageUrl, url);
            Assert.Equal(result.ThumbnailUrl, url);
        }

        /// <summary>
        ///     Tests the value of the <see cref="EmbedBuilder.Length"/> property when there are no fields set.
        /// </summary>
        [Fact]
        public void Length_Empty()
        {
            var empty = new EmbedBuilder();
            Assert.Equal(0, empty.Length);
        }

        /// <summary>
        ///     Tests the value of the <see cref="EmbedBuilder.Length"/> property when all fields are set.
        /// </summary>
        [Fact]
        public void Length()
        {
            var e = new EmbedBuilder()
                .WithAuthor(Name, Icon, Url)
                .WithColor(Color.Blue)
                .WithDescription("This is the test description.")
                .WithFooter("This is the footer", Url)
                .WithImageUrl(Url)
                .WithThumbnailUrl(Url)
                .WithTimestamp(DateTimeOffset.MinValue)
                .WithTitle("This is the title")
                .WithUrl(Url)
                .AddField("Field 1", "Inline", true)
                .AddField("Field 2", "Not Inline", false);
            Assert.Equal(100, e.Length);
        }

        /// <summary>
        ///     Tests the behavior of <see cref="EmbedBuilder.WithCurrentTimestamp"/>.
        /// </summary>
        [Fact]
        public void WithCurrentTimestamp()
        {
            var e = new EmbedBuilder()
                .WithCurrentTimestamp();
            // ensure within a second of accuracy
            Assert.Equal(DateTime.UtcNow, e.Timestamp.Value.UtcDateTime, TimeSpan.FromSeconds(1));
        }

        /// <summary>
        ///     Tests the behavior of <see cref="EmbedBuilder.WithColor(Color)"/>.
        /// </summary>
        [Fact]
        public void WithColor()
        {
            // use WithColor
            var e = new EmbedBuilder().WithColor(Color.Red);
            Assert.Equal(Color.Red.RawValue, e.Color.Value.RawValue);
        }

        /// <summary>
        ///     Tests the behavior of <see cref="EmbedBuilder.WithFooter(Action{EmbedFooterBuilder})"/>
        /// </summary>
        [Fact]
        public void WithFooter_ActionFooterBuilder()
        {
            var e = new EmbedBuilder()
                .WithFooter(x =>
                {
                    x.IconUrl = Url;
                    x.Text = Name;
                });
            Assert.Equal(Url, e.Footer.IconUrl);
            Assert.Equal(Name, e.Footer.Text);
        }

        /// <summary>
        ///     Tests the behavior of <see cref="EmbedBuilder.WithFooter(EmbedFooterBuilder)"/>
        /// </summary>
        [Fact]
        public void WithFooter_FooterBuilder()
        {
            var footer = new EmbedFooterBuilder()
            {
                IconUrl = Url,
                Text = Name
            };
            var e = new EmbedBuilder()
                .WithFooter(footer);
            Assert.Equal(Url, e.Footer.IconUrl);
            Assert.Equal(Name, e.Footer.Text);
            // use the property
            e = new EmbedBuilder
            {
                Footer = footer
            };
            Assert.Equal(Url, e.Footer.IconUrl);
            Assert.Equal(Name, e.Footer.Text);
        }

        /// <summary>
        ///     Tests the behavior of <see cref="EmbedBuilder.WithFooter(string, string)"/>
        /// </summary>
        [Fact]
        public void WithFooter_Strings()
        {
            var e = new EmbedBuilder()
                .WithFooter(Name, Url);
            Assert.Equal(Url, e.Footer.IconUrl);
            Assert.Equal(Name, e.Footer.Text);
        }

        /// <summary>
        ///     Tests the behavior of <see cref="EmbedFooterBuilder"/>.
        /// </summary>
        [Fact]
        public void EmbedFooterBuilder()
        {
            var footer = new EmbedFooterBuilder()
                .WithIconUrl(Url)
                .WithText(Name);
            Assert.Equal(Url, footer.IconUrl);
            Assert.Equal(Name, footer.Text);
        }
        /// <summary>
        ///     Tests that invalid text throws an <see cref="ArgumentException"/>.
        /// </summary>
        [Fact]
        public void EmbedFooterBuilder_InvalidText()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new EmbedFooterBuilder().WithText(new string('a', 2049));
            });
        }
        [Fact]
        public void AddField_Strings()
        {
            var e = new EmbedBuilder()
                .AddField("name", "value", true);
            Assert.Equal("name", e.Fields[0].Name);
            Assert.Equal("value", e.Fields[0].Value);
            Assert.True(e.Fields[0].IsInline);
        }
        [Fact]
        public void AddField_EmbedFieldBuilder()
        {
            var field = new EmbedFieldBuilder()
                .WithIsInline(true)
                .WithValue("value")
                .WithName("name");
            var e = new EmbedBuilder()
                .AddField(field);
            Assert.Equal("name", e.Fields[0].Name);
            Assert.Equal("value", e.Fields[0].Value);
            Assert.True(e.Fields[0].IsInline);
        }
        [Fact]
        public void AddField_ActionEmbedFieldBuilder()
        {
            var e = new EmbedBuilder()
                .AddField(x => x
                    .WithName("name")
                    .WithValue("value")
                    .WithIsInline(true));
            Assert.Equal("name", e.Fields[0].Name);
            Assert.Equal("value", e.Fields[0].Value);
            Assert.True(e.Fields[0].IsInline);
        }
        [Fact]
        public void AddField_TooManyFields()
        {
            var e = new EmbedBuilder();
            for (var i = 0; i < 25; i++)
            {
                e = e.AddField("name", "value", false);
            }
            Assert.Throws<ArgumentException>(() =>
            {
                e = e.AddField("name", "value", false);
            });
        }
        [Fact]
        public void EmbedFieldBuilder()
        {
            var e = new EmbedFieldBuilder()
                .WithIsInline(true)
                .WithName("name")
                .WithValue("value");
            Assert.Equal("name", e.Name);
            Assert.Equal("value", e.Value);
            Assert.True(e.IsInline);
            // use the properties
            e = new EmbedFieldBuilder
            {
                IsInline = true,
                Name = "name",
                Value = "value"
            };
            Assert.Equal("name", e.Name);
            Assert.Equal("value", e.Value);
            Assert.True(e.IsInline);
        }
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        // 257 chars
        [InlineData("jVyLChmA7aBZozXQuZ3VDEcwW6zOq0nteOVYBZi31ny73rpXfSSBXR4Jw6FiplDKQseKskwRMuBZkUewrewqAbkBZpslHirvC5nEzRySoDIdTRnkVvTXZUXg75l3bQCjuuHxDd6DfrY8ihd6yZX1Y0XFeg239YBcYV4TpL9uQ8H3HFYxrWhLlG2PRVjUmiglP5iXkawszNwMVm1SZ5LZT4jkMZHxFegVi7170d16iaPWOovu50aDDHy087XBtLKVa")]
        // 257 chars of whitespace
        [InlineData("                                                                                                                                                                                                                                                                 ")]
        public void EmbedFieldBuilder_InvalidName(string name)
        {
            Assert.Throws<ArgumentException>(() => new EmbedFieldBuilder().WithName(name));
        }
        [Fact]
        public void EmbedFieldBuilder_InvalidValue()
        {
            IEnumerable<string> GetInvalidValue()
            {
                yield return null;
                yield return string.Empty;
                yield return " ";
                yield return new string('a', 1025);
            };
            foreach (var v in GetInvalidValue())
                Assert.Throws<ArgumentException>(() => new EmbedFieldBuilder().WithValue(v));
        }
    }
}
