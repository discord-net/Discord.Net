using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord
{
    public class EmbedBuilder
    {
        private string _title;
        private string _description;
        private string _url;
        private EmbedImage? _image;
        private EmbedThumbnail? _thumbnail;
        private List<EmbedFieldBuilder> _fields;

        public const int MaxFieldCount = 25;
        public const int MaxTitleLength = 256;
        public const int MaxDescriptionLength = 2048;
        public const int MaxEmbedLength = 6000;

        public EmbedBuilder()
        {
            Fields = new List<EmbedFieldBuilder>();
        }

        public string Title
        {
            get => _title;
            set
            {
                if (value?.Length > MaxTitleLength) throw new ArgumentException($"Title length must be less than or equal to {MaxTitleLength}.", nameof(Title));
                _title = value;
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                if (value?.Length > MaxDescriptionLength) throw new ArgumentException($"Description length must be less than or equal to {MaxDescriptionLength}.", nameof(Description));
                _description = value;
            }
        }

        public string Url
        {
            get => _url;
            set
            {
                if (!value.IsNullOrUri()) throw new ArgumentException("Url must be a well-formed URI", nameof(Url));
                _url = value;
            }
        }
        public string ThumbnailUrl
        {
            get => _thumbnail?.Url;
            set
            {
                if (!value.IsNullOrUri()) throw new ArgumentException("Url must be a well-formed URI", nameof(ThumbnailUrl));
                _thumbnail = new EmbedThumbnail(value, null, null, null);
            }
        }
        public string ImageUrl
        {
            get => _image?.Url;
            set
            {
                if (!value.IsNullOrUri()) throw new ArgumentException("Url must be a well-formed URI", nameof(ImageUrl));
                _image = new EmbedImage(value, null, null, null);
            }
        }
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

        public DateTimeOffset? Timestamp { get; set; }
        public Color? Color { get; set; }
        public EmbedAuthorBuilder Author { get; set; }
        public EmbedFooterBuilder Footer { get; set; }

        public int Length
        {
            get
            {
                int titleLength = Title?.Length ?? 0;
                int authorLength = Author?.Name?.Length ?? 0;
                int descriptionLength = Description?.Length ?? 0;
                int footerLength = Footer?.Text?.Length ?? 0;
                int fieldSum = Fields.Sum(f => f.Name.Length + f.Value.ToString().Length);

                return titleLength + authorLength + descriptionLength + footerLength + fieldSum;
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

        public EmbedBuilder AddField(string name, object value, bool inline = false)
        {
            var field = new EmbedFieldBuilder()
                .WithIsInline(inline)
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
            AddField(field);
            return this;
        }

        public Embed Build()
        {
            if (Length > MaxEmbedLength)
                throw new InvalidOperationException($"Total embed length must be less than or equal to {MaxEmbedLength}");

            var fields = ImmutableArray.CreateBuilder<EmbedField>(Fields.Count);
            for (int i = 0; i < Fields.Count; i++)
                fields.Add(Fields[i].Build());

            return new Embed(EmbedType.Rich, Title, Description, Url, Timestamp, Color, _image, null, Author?.Build(), Footer?.Build(), null, _thumbnail, fields.ToImmutable());
        }
    }

    public class EmbedFieldBuilder
    {
        private string _name;
        private string _value;
        public const int MaxFieldNameLength = 256;
        public const int MaxFieldValueLength = 1024;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Field name must not be null, empty or entirely whitespace.", nameof(Name));
                if (value.Length > MaxFieldNameLength) throw new ArgumentException($"Field name length must be less than or equal to {MaxFieldNameLength}.", nameof(Name));
                _name = value;
            }
        }

        public object Value
        {
            get => _value;
            set
            {
                var stringValue = value?.ToString();
                if (string.IsNullOrEmpty(stringValue)) throw new ArgumentException("Field value must not be null or empty.", nameof(Value));
                if (stringValue.Length > MaxFieldValueLength) throw new ArgumentException($"Field value length must be less than or equal to {MaxFieldValueLength}.", nameof(Value));
                _value = stringValue;
            }
        }
        public bool IsInline { get; set; }

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
            => new EmbedField(Name, Value.ToString(), IsInline);
    }

    public class EmbedAuthorBuilder
    {
        private string _name;
        private string _url;
        private string _iconUrl;
        public const int MaxAuthorNameLength = 256;

        public string Name
        {
            get => _name;
            set
            {
                if (value?.Length > MaxAuthorNameLength) throw new ArgumentException($"Author name length must be less than or equal to {MaxAuthorNameLength}.", nameof(Name));
                _name = value;
            }
        }
        public string Url
        {
            get => _url;
            set
            {
                if (!value.IsNullOrUri()) throw new ArgumentException("Url must be a well-formed URI", nameof(Url));
                _url = value;
            }
        }
        public string IconUrl
        {
            get => _iconUrl;
            set
            {
                if (!value.IsNullOrUri()) throw new ArgumentException("Url must be a well-formed URI", nameof(IconUrl));
                _iconUrl = value;
            }
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
            => new EmbedAuthor(Name, Url, IconUrl, null);
    }

    public class EmbedFooterBuilder
    {
        private string _text;
        private string _iconUrl;

        public const int MaxFooterTextLength = 2048;

        public string Text
        {
            get => _text;
            set
            {
                if (value?.Length > MaxFooterTextLength) throw new ArgumentException($"Footer text length must be less than or equal to {MaxFooterTextLength}.", nameof(Text));
                _text = value;
            }
        }
        public string IconUrl
        {
            get => _iconUrl;
            set
            {
                if (!value.IsNullOrUri()) throw new ArgumentException("Url must be a well-formed URI", nameof(IconUrl));
                _iconUrl = value;
            }
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
            => new EmbedFooter(Text, IconUrl, null);
    }
}
