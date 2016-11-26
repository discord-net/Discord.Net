using System;
using System.Collections.Generic;
using Embed = Discord.API.Embed;
using Field = Discord.API.EmbedField;
using Author = Discord.API.EmbedAuthor;
using Footer = Discord.API.EmbedFooter;
using Thumbnail = Discord.API.EmbedThumbnail;
using Image = Discord.API.EmbedImage;

namespace Discord
{
    public class EmbedBuilder
    {
        private readonly Embed _model;
        private readonly List<Field> _fields;

        public EmbedBuilder()
        {
            _model = new Embed { Type = "rich" };
            _fields = new List<Field>();
        }

        public string Title { get { return _model.Title; } set { _model.Title = value; } }
        public string Description { get { return _model.Description; } set { _model.Description = value; } }
        public string Url { get { return _model.Url; } set { _model.Url = value; } }
        public string ThumbnailUrl { get; set; }
        public string ImageUrl { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public Color? Color { get { return _model.Color.HasValue ? new Color(_model.Color.Value) : (Color?)null; } set { _model.Color = value?.RawValue; } }
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
            ImageUrl = ImageUrl;
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

        public EmbedBuilder AddField(Action<EmbedFieldBuilder> action)
        {
            var field = new EmbedFieldBuilder();
            action(field);
            _fields.Add(field.ToModel());
            return this;
        }

        internal Embed Build()
        {
            _model.Author = Author?.ToModel();
            _model.Footer = Footer?.ToModel();
            _model.Timestamp = Timestamp?.ToUniversalTime();
            _model.Thumbnail = ThumbnailUrl != null ? new Thumbnail { Url = ThumbnailUrl } : null;
            _model.Image = ImageUrl != null ? new Image { Url = ImageUrl } : null;
            _model.Fields = _fields.ToArray();
            return _model;
        }
    }

    public class EmbedFieldBuilder
    {
        private readonly Field _model;

        public string Name { get { return _model.Name; } set { _model.Name = value; } }
        public string Value { get { return _model.Value; } set { _model.Value = value; } }
        public bool IsInline { get { return _model.Inline; } set { _model.Inline = value; } }

        public EmbedFieldBuilder()
        {
            _model = new Field();
        }

        public EmbedFieldBuilder WithName(string name)
        {
            Name = name;
            return this;
        }
        public EmbedFieldBuilder WithValue(string value)
        {
            Value = value;
            return this;
        }
        public EmbedFieldBuilder WithIsInline(bool isInline)
        {
            IsInline = isInline;
            return this;
        }

        internal Field ToModel() => _model;
    }

    public class EmbedAuthorBuilder
    {
        private readonly Author _model;

        public string Name { get { return _model.Name; } set { _model.Name = value; } }
        public string Url { get { return _model.Url; } set { _model.Url = value; } }
        public string IconUrl { get { return _model.IconUrl; } set { _model.IconUrl = value; } }

        public EmbedAuthorBuilder()
        {
            _model = new Author();
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

        internal Author ToModel() => _model;
    }

    public class EmbedFooterBuilder
    {
        private readonly Footer _model;

        public string Text { get { return _model.Text; } set { _model.Text = value; } }
        public string IconUrl { get { return _model.IconUrl; } set { _model.IconUrl = value; } }

        public EmbedFooterBuilder()
        {
            _model = new Footer();
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

        internal Footer ToModel() => _model;
    }
}
