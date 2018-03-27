using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord
{
    /// <summary> A builder for creating an <see cref="Embed"/> to be sent. </summary>
    public class EmbedBuilder
    {
        private readonly Embed _embed;

        /// <summary> The maximum number of fields allowed by Discord. </summary>
        public const int MaxFieldCount = 25;
        /// <summary> The maximum length of title allowed by Discord. </summary>
        public const int MaxTitleLength = 256;
        /// <summary> The maximum length of description allowed by Discord. </summary>
        public const int MaxDescriptionLength = 2048;
        /// <summary> The maximum length of total characters allowed by Discord. </summary>
        public const int MaxEmbedLength = 6000; // user bot limit is 2000, but we don't validate that here.

        /// <summary> Creates a new <see cref="EmbedBuilder"/>. </summary>
        public EmbedBuilder()
        {
            _embed = new Embed(EmbedType.Rich);
            Fields = new List<EmbedFieldBuilder>();
        }

        /// <summary> Gets or sets the title of an <see cref="Embed"/>. </summary>
        public string Title
        {
            get => _embed.Title;
            set
            {
                if (value?.Length > MaxTitleLength) throw new ArgumentException($"Title length must be less than or equal to {MaxTitleLength}.", nameof(Title));
                _embed.Title = value;
            }
        }

        /// <summary> Gets or sets the description of an <see cref="Embed"/>. </summary>
        public string Description
        {
            get => _embed.Description;
            set
            {
                if (value?.Length > MaxDescriptionLength) throw new ArgumentException($"Description length must be less than or equal to {MaxDescriptionLength}.", nameof(Description));
                _embed.Description = value;
            }
        }

        /// <summary> Gets or sets the URL of an <see cref="Embed"/>. </summary>
        public string Url
        {
            get => _embed.Url;
            set
            {
                if (!value.IsNullOrUri()) throw new ArgumentException("Url must be a well-formed URI", nameof(Url));
                _embed.Url = value;
            }
        }
        /// <summary> Gets or sets the thumbnail URL of an <see cref="Embed"/>. </summary>
        public string ThumbnailUrl
        {
            get => _embed.Thumbnail?.Url;
            set
            {
                if (!value.IsNullOrUri()) throw new ArgumentException("Url must be a well-formed URI", nameof(ThumbnailUrl));
                _embed.Thumbnail = new EmbedThumbnail(value, null, null, null);
            }
        }
        /// <summary> Gets or sets the image URL of an <see cref="Embed"/>. </summary>
        public string ImageUrl
        {
            get => _embed.Image?.Url;
            set
            {
                if (!value.IsNullOrUri()) throw new ArgumentException("Url must be a well-formed URI", nameof(ImageUrl));
                _embed.Image = new EmbedImage(value, null, null, null);
            }
        }
        /// <summary> Gets or sets the timestamp of an <see cref="Embed"/>. </summary>
        public DateTimeOffset? Timestamp { get => _embed.Timestamp; set { _embed.Timestamp = value; } }
        /// <summary> Gets or sets the sidebar color of an <see cref="Embed"/>. </summary>
        public Color? Color { get => _embed.Color; set { _embed.Color = value; } }

        /// <summary> Gets or sets the <see cref="EmbedAuthorBuilder"/> of an <see cref="Embed"/>. </summary>
        public EmbedAuthorBuilder Author { get; set; }
        /// <summary> Gets or sets the <see cref="EmbedFooterBuilder"/> of an <see cref="Embed"/>. </summary>
        public EmbedFooterBuilder Footer { get; set; }
        private List<EmbedFieldBuilder> _fields;
        /// <summary> Gets or sets the list of <see cref="EmbedFieldBuilder"/> of an <see cref="Embed"/>. </summary>
        public List<EmbedFieldBuilder> Fields
        {
            get => _fields;
            set
            {

                if (value == null) throw new ArgumentNullException("Cannot set an embed builder's fields collection to null", nameof(Fields));
                if (value.Count > MaxFieldCount) throw new ArgumentException($"Field count must be less than or equal to {MaxFieldCount}.", nameof(Fields));
                _fields = value;
            }
        }

        /// <summary> Sets the title of an <see cref="Embed"/>. </summary>
        /// <param name="title"> The title to be set. </param>
        public EmbedBuilder WithTitle(string title)
        {
            Title = title;
            return this;
        }
        /// <summary> Sets the description of an <see cref="Embed"/>. </summary>
        /// <param name="description"> The description to be set. </param>
        public EmbedBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }
        /// <summary> Sets the URL of an <see cref="Embed"/>. </summary>
        /// <param name="url"> The URL to be set. </param>
        public EmbedBuilder WithUrl(string url)
        {
            Url = url;
            return this;
        }
        /// <summary> Sets the thumbnail URL of an <see cref="Embed"/>. </summary>
        /// <param name="thumbnailUrl"> The thumbnail URL to be set. </param>
        public EmbedBuilder WithThumbnailUrl(string thumbnailUrl)
        {
            ThumbnailUrl = thumbnailUrl;
            return this;
        }
        /// <summary> Sets the image URL of an <see cref="Embed"/>. </summary>
        /// <param name="imageUrl"> The image URL to be set. </param>
        public EmbedBuilder WithImageUrl(string imageUrl)
        {
            ImageUrl = imageUrl;
            return this;
        }
        /// <summary> Sets the timestamp of an <see cref="Embed"/> to the current time. </summary>
        public EmbedBuilder WithCurrentTimestamp()
        {
            Timestamp = DateTimeOffset.UtcNow;
            return this;
        }
        /// <summary> Sets the timestamp of an <see cref="Embed"/>. </summary>
        /// <param name="dateTimeOffset"> The timestamp to be set. </param>
        public EmbedBuilder WithTimestamp(DateTimeOffset dateTimeOffset)
        {
            Timestamp = dateTimeOffset;
            return this;
        }
        /// <summary> Sets the sidebar color of an <see cref="Embed"/>. </summary>
        /// <param name="color"> The color to be set. </param>
        public EmbedBuilder WithColor(Color color)
        {
            Color = color;
            return this;
        }

        /// <summary> Sets the <see cref="EmbedAuthorBuilder"/> of an <see cref="Embed"/>. </summary>
        /// <param name="author"> The author builder class containing the author field properties. </param>
        public EmbedBuilder WithAuthor(EmbedAuthorBuilder author)
        {
            Author = author;
            return this;
        }
        /// <summary> Sets the author field of an <see cref="Embed"/> with the provided properties. </summary>
        /// <param name="action"> The delegate containing the author field properties. </param>
        public EmbedBuilder WithAuthor(Action<EmbedAuthorBuilder> action)
        {
            var author = new EmbedAuthorBuilder();
            action(author);
            Author = author;
            return this;
        }
        /// <summary> Sets the author field of an <see cref="Embed"/> with the provided name, icon URL, and URL. </summary>
        /// <param name="name"> The title of the author field. </param>
        /// <param name="iconUrl"> The icon URL of the author field. </param>
        /// <param name="url"> The URL of the author field. </param>
        public EmbedBuilder WithAuthor(string name, string iconUrl = null, string url = null)
        {
            var author = new EmbedAuthorBuilder
            {
                Name = name,
                IconUrl = iconUrl,
                Url = url
            };
            Author = author;
            return this;
        }
        /// <summary> Sets the <see cref="EmbedFooterBuilder"/> of an <see cref="Embed"/>. </summary>
        /// <param name="footer"> The footer builder class containing the footer field properties. </param>
        public EmbedBuilder WithFooter(EmbedFooterBuilder footer)
        {
            Footer = footer;
            return this;
        }
        /// <summary> Sets the footer field of an <see cref="Embed"/> with the provided properties. </summary>
        /// <param name="action"> The delegate containing the footer field properties. </param>
        public EmbedBuilder WithFooter(Action<EmbedFooterBuilder> action)
        {
            var footer = new EmbedFooterBuilder();
            action(footer);
            Footer = footer;
            return this;
        }
        /// <summary> Sets the footer field of an <see cref="Embed"/> with the provided name, icon URL. </summary>
        /// <param name="text"> The title of the footer field. </param>
        /// <param name="iconUrl"> The icon URL of the footer field. </param>
        public EmbedBuilder WithFooter(string text, string iconUrl = null)
        {
            var footer = new EmbedFooterBuilder
            {
                Text = text,
                IconUrl = iconUrl
            };
            Footer = footer;
            return this;
        }

        /// <summary> Adds an <see cref="Embed"/> field with the provided name and value. </summary>
        /// <param name="name"> The title of the field. </param>
        /// <param name="value"> The value of the field. </param>
        /// <param name="inline"> Indicates whether the field is in-line or not. </param>
        public EmbedBuilder AddField(string name, object value, bool inline = false)
        {
            var field = new EmbedFieldBuilder()
                .WithIsInline(inline)
                .WithName(name)
                .WithValue(value);
            AddField(field);
            return this;
        }

        /// <summary> Adds a field with the provided <see cref="EmbedFieldBuilder"/> to an <see cref="Embed"/>. </summary>
        /// <param name="field"> The field builder class containing the field properties. </param>
        public EmbedBuilder AddField(EmbedFieldBuilder field)
        {
            if (Fields.Count >= MaxFieldCount)
            {
                throw new ArgumentException($"Field count must be less than or equal to {MaxFieldCount}.", nameof(field));
            }

            Fields.Add(field);
            return this;
        }
        /// <summary> Adds an <see cref="Embed"/> field with the provided properties. </summary>
        /// <param name="action"> The delegate containing the field properties. </param>
        public EmbedBuilder AddField(Action<EmbedFieldBuilder> action)
        {
            var field = new EmbedFieldBuilder();
            action(field);
            this.AddField(field);
            return this;
        }

        /// <summary> Builds the <see cref="Embed"/> into a Rich Embed format. </summary>
        /// <returns> The built embed object. </returns>
        public Embed Build()
        {
            _embed.Footer = Footer?.Build();
            _embed.Author = Author?.Build();
            var fields = ImmutableArray.CreateBuilder<EmbedField>(Fields.Count);
            for (int i = 0; i < Fields.Count; i++)
                fields.Add(Fields[i].Build());
            _embed.Fields = fields.ToImmutable();

            if (_embed.Length > MaxEmbedLength)
            {
                throw new InvalidOperationException($"Total embed length must be less than or equal to {MaxEmbedLength}");
            }

            return _embed;
        }
    }

    public class EmbedFieldBuilder
    {
        private EmbedField _field;

        public const int MaxFieldNameLength = 256;
        public const int MaxFieldValueLength = 1024;

        public string Name
        {
            get => _field.Name;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"Field name must not be null, empty or entirely whitespace.", nameof(Name));
                if (value.Length > MaxFieldNameLength) throw new ArgumentException($"Field name length must be less than or equal to {MaxFieldNameLength}.", nameof(Name));
                _field.Name = value;
            }
        }

        public object Value
        {
            get => _field.Value;
            set
            {
                var stringValue = value?.ToString();
                if (string.IsNullOrEmpty(stringValue)) throw new ArgumentException($"Field value must not be null or empty.", nameof(Value));
                if (stringValue.Length > MaxFieldValueLength) throw new ArgumentException($"Field value length must be less than or equal to {MaxFieldValueLength}.", nameof(Value));
                _field.Value = stringValue;
            }
        }
        public bool IsInline { get => _field.Inline; set { _field.Inline = value; } }

        public EmbedFieldBuilder()
        {
            _field = new EmbedField();
        }

        public EmbedFieldBuilder WithName(string name)
        {
            Name = name;
            return this;
        }
        public EmbedFieldBuilder WithValue(object value)
        {
            Value = value;
            return this;
        }
        public EmbedFieldBuilder WithIsInline(bool isInline)
        {
            IsInline = isInline;
            return this;
        }

        public EmbedField Build()
            => _field;
    }

    public class EmbedAuthorBuilder
    {
        private EmbedAuthor _author;

        public const int MaxAuthorNameLength = 256;

        public string Name
        {
            get => _author.Name;
            set
            {
                if (value?.Length > MaxAuthorNameLength) throw new ArgumentException($"Author name length must be less than or equal to {MaxAuthorNameLength}.", nameof(Name));
                _author.Name = value;
            }
        }
        public string Url
        {
            get => _author.Url;
            set
            {
                if (!value.IsNullOrUri()) throw new ArgumentException("Url must be a well-formed URI", nameof(Url));
                _author.Url = value;
            }
        }
        public string IconUrl
        {
            get => _author.IconUrl;
            set
            {
                if (!value.IsNullOrUri()) throw new ArgumentException("Url must be a well-formed URI", nameof(IconUrl));
                _author.IconUrl = value;
            }
        }

        public EmbedAuthorBuilder()
        {
            _author = new EmbedAuthor();
        }

        public EmbedAuthorBuilder WithName(string name)
        {
            Name = name;
            return this;
        }
        public EmbedAuthorBuilder WithUrl(string url)
        {
            Url = url;
            return this;
        }
        public EmbedAuthorBuilder WithIconUrl(string iconUrl)
        {
            IconUrl = iconUrl;
            return this;
        }

        public EmbedAuthor Build()
            => _author;
    }

    public class EmbedFooterBuilder
    {
        private EmbedFooter _footer;

        public const int MaxFooterTextLength = 2048;

        public string Text
        {
            get => _footer.Text;
            set
            {
                if (value?.Length > MaxFooterTextLength) throw new ArgumentException($"Footer text length must be less than or equal to {MaxFooterTextLength}.", nameof(Text));
                _footer.Text = value;
            }
        }
        public string IconUrl
        {
            get => _footer.IconUrl;
            set
            {
                if (!value.IsNullOrUri()) throw new ArgumentException("Url must be a well-formed URI", nameof(IconUrl));
                _footer.IconUrl = value;
            }
        }

        public EmbedFooterBuilder()
        {
            _footer = new EmbedFooter();
        }

        public EmbedFooterBuilder WithText(string text)
        {
            Text = text;
            return this;
        }
        public EmbedFooterBuilder WithIconUrl(string iconUrl)
        {
            IconUrl = iconUrl;
            return this;
        }

        public EmbedFooter Build()
            => _footer;
    }
}
