using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord
{
    public class EmbedBuilder
    {
        private readonly Embed _embed;

        public const int MaxFieldCount = 25;
        public const int MaxTitleLength = 256;
        public const int MaxDescriptionLength = 2048;
        public const int MaxEmbedLength = 6000; // user bot limit is 2000, but we don't validate that here.

        public EmbedBuilder()
        {
            _embed = new Embed(EmbedType.Rich);
            Fields = new List<EmbedFieldBuilder>();
        }

        public string Title
        {
            get => _embed.Title;
            set
            {
                if (value?.Length > MaxTitleLength) throw new ArgumentException($"Title length must be less than or equal to {MaxTitleLength}.", nameof(Title));
                _embed.Title = value;
            }
        }

        public string Description
        {
            get => _embed.Description;
            set
            {
                if (value?.Length > MaxDescriptionLength) throw new ArgumentException($"Description length must be less than or equal to {MaxDescriptionLength}.", nameof(Description));
                _embed.Description = value;
            }
        }

        public string Url
        {
            get => _embed.Url;
            set
            {
                if (!value.IsNullOrUri()) throw new ArgumentException("Url must be a well-formed URI", nameof(Url));
                _embed.Url = value;
            }
        }
        public string ThumbnailUrl
        {
            get => _embed.Thumbnail?.Url;
            set
            {
                if (!value.IsNullOrUri()) throw new ArgumentException("Url must be a well-formed URI", nameof(ThumbnailUrl));
                _embed.Thumbnail = new EmbedThumbnail(value, null, null, null);
            }
        }
        public string ImageUrl
        {
            get => _embed.Image?.Url;
            set
            {
                if (!value.IsNullOrUri()) throw new ArgumentException("Url must be a well-formed URI", nameof(ImageUrl));
                _embed.Image = new EmbedImage(value, null, null, null);
            }
        }
        public DateTimeOffset? Timestamp { get => _embed.Timestamp; set { _embed.Timestamp = value; } }
        public Color? Color { get => _embed.Color; set { _embed.Color = value; } }

        public EmbedAuthorBuilder Author { get; set; }
        public EmbedFooterBuilder Footer { get; set; }
        private List<EmbedFieldBuilder> _fields;
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

        public EmbedBuilder WithTitle(string title)
        {
            Title = title;
            return this;
        }
        public EmbedBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }
        public EmbedBuilder WithUrl(string url)
        {
            Url = url;
            return this;
        }
        public EmbedBuilder WithThumbnailUrl(string thumbnailUrl)
        {
            ThumbnailUrl = thumbnailUrl;
            return this;
        }
        public EmbedBuilder WithImageUrl(string imageUrl)
        {
            ImageUrl = imageUrl;
            return this;
        }
        public EmbedBuilder WithCurrentTimestamp()
        {
            Timestamp = DateTimeOffset.UtcNow;
            return this;
        }
        public EmbedBuilder WithTimestamp(DateTimeOffset dateTimeOffset)
        {
            Timestamp = dateTimeOffset;
            return this;
        }
        public EmbedBuilder WithColor(Color color)
        {
            Color = color;
            return this;
        }

        public EmbedBuilder WithAuthor(EmbedAuthorBuilder author)
        {
            Author = author;
            return this;
        }
        public EmbedBuilder WithAuthor(Action<EmbedAuthorBuilder> action)
        {
            var author = new EmbedAuthorBuilder();
            action(author);
            Author = author;
            return this;
        }
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
        public EmbedBuilder WithFooter(EmbedFooterBuilder footer)
        {
            Footer = footer;
            return this;
        }
        public EmbedBuilder WithFooter(Action<EmbedFooterBuilder> action)
        {
            var footer = new EmbedFooterBuilder();
            action(footer);
            Footer = footer;
            return this;
        }
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

        public EmbedBuilder AddField(string name, object value)
        {
            var field = new EmbedFieldBuilder()
                .WithIsInline(false)
                .WithName(name)
                .WithValue(value);
            AddField(field);
            return this;
        }
        public EmbedBuilder AddInlineField(string name, object value)
        {
            var field = new EmbedFieldBuilder()
                .WithIsInline(true)
                .WithName(name)
                .WithValue(value);
            AddField(field);
            return this;
        }
        public EmbedBuilder AddField(EmbedFieldBuilder field)
        {
            if (Fields.Count >= MaxFieldCount)
            {
                throw new ArgumentException($"Field count must be less than or equal to {MaxFieldCount}.", nameof(field));
            }

            Fields.Add(field);
            return this;
        }
        public EmbedBuilder AddField(Action<EmbedFieldBuilder> action)
        {
            var field = new EmbedFieldBuilder();
            action(field);
            this.AddField(field);
            return this;
        }
        public EmbedBuilder AddField(string title, string text, bool inline = false)
        {
            var field = new EmbedFieldBuilder
            {
                Name = title,
                Value = text,
                IsInline = inline
            };
            _fields.Add(field);
            return this;
        }

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
        public static implicit operator Embed(EmbedBuilder builder) => builder?.Build();
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
                if (string.IsNullOrEmpty(value)) throw new ArgumentException($"Field name must not be null or empty.", nameof(Name));
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
