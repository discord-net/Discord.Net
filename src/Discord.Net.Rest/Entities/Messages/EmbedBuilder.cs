using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord
{
    public class EmbedBuilder
    {
        private readonly Embed _embed;
        private readonly List<EmbedFieldBuilder> _fields;

        public EmbedBuilder()
        {
            _embed = new Embed("rich");
            _fields = new List<EmbedFieldBuilder>();
        }

        public string Title { get { return _embed.Title; } set { _embed.Title = value; } }
        public string Description { get { return _embed.Description; } set { _embed.Description = value; } }
        public string Url { get { return _embed.Url; } set { _embed.Url = value; } }
        public string ThumbnailUrl { get { return _embed.Thumbnail?.Url; } set { _embed.Thumbnail = new EmbedThumbnail(value, null, null, null); } }
        public string ImageUrl { get { return _embed.Image?.Url; } set { _embed.Image = new EmbedImage(value, null, null, null); } }
        public DateTimeOffset? Timestamp { get { return _embed.Timestamp; } set { _embed.Timestamp = value; } }
        public Color? Color { get { return _embed.Color; } set { _embed.Color = value; } }

        public EmbedAuthorBuilder Author { get; set; }
        public EmbedFooterBuilder Footer { get; set; }

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

        public EmbedBuilder AddField(string name, object value)
        {
            var field = new EmbedFieldBuilder()
                .WithIsInline(false)
                .WithName(name)
                .WithValue(value);
            _fields.Add(field);
            return this;
        }
        public EmbedBuilder AddInlineField(string name, object value)
        {
            var field = new EmbedFieldBuilder()
                .WithIsInline(true)
                .WithName(name)
                .WithValue(value);
            _fields.Add(field);
            return this;
        }
        public EmbedBuilder AddField(EmbedFieldBuilder field)
        {
            _fields.Add(field);
            return this;
        }
        public EmbedBuilder AddField(Action<EmbedFieldBuilder> action)
        {
            var field = new EmbedFieldBuilder();
            action(field);
            _fields.Add(field);
            return this;
        }

        public Embed Build()
        {
            _embed.Footer = Footer?.Build();
            _embed.Author = Author?.Build();
            var fields = ImmutableArray.CreateBuilder<EmbedField>(_fields.Count);
            for (int i = 0; i < _fields.Count; i++)
                fields.Add(_fields[i].Build());
            _embed.Fields = fields.ToImmutable();
            return _embed;
        }
        public static implicit operator Embed(EmbedBuilder builder) => builder?.Build();
    }

    public class EmbedFieldBuilder
    {
        private EmbedField _field;

        public string Name { get { return _field.Name; } set { _field.Name = value; } }
        public object Value { get { return _field.Value; } set { _field.Value = value.ToString(); } }
        public bool IsInline { get { return _field.Inline; } set { _field.Inline = value; } }

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

        public string Name { get { return _author.Name; } set { _author.Name = value; } }
        public string Url { get { return _author.Url; } set { _author.Url = value; } }
        public string IconUrl { get { return _author.IconUrl; } set { _author.IconUrl = value; } }

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

        public string Text { get { return _footer.Text; } set { _footer.Text = value; } }
        public string IconUrl { get { return _footer.IconUrl; } set { _footer.IconUrl = value; } }

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
