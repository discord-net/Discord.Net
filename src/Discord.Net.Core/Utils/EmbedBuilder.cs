using System;
using System.Collections.Generic;
using Embed = Discord.API.Embed;
using Field = Discord.API.EmbedField;
using Author = Discord.API.EmbedAuthor;
using Footer = Discord.API.EmbedFooter;

namespace Discord
{
    public class EmbedBuilder
    {
        private Embed embed = new Embed();
        List<Field> fields = new List<Field>();

        public EmbedBuilder()
        {
            embed.Type = "rich";
        }

        public EmbedBuilder Title(string title)
        {
            embed.Title = title;
            return this;
        }
        public EmbedBuilder Description(string description)
        {
            embed.Description = description;
            return this;
        }
        public EmbedBuilder Url(string url)
        {
            embed.Url = url;
            return this;
        }
        public EmbedBuilder Color(Color color)
        {
            embed.Color = color.RawValue;
            return this;
        }
        public EmbedBuilder Field(Func<EmbedFieldBuilder, EmbedFieldBuilder> builder)
        {
            fields.Add(builder(new EmbedFieldBuilder()).Build());
            return this;
        }
        public EmbedBuilder Author(Func<EmbedAuthorBuilder, EmbedAuthorBuilder> builder)
        {
            embed.Author = builder(new EmbedAuthorBuilder()).Build();
            return this;
        }
        public EmbedBuilder Footer(Func<EmbedFooterBuilder, EmbedFooterBuilder> builder)
        {
            embed.Footer = builder(new EmbedFooterBuilder()).Build();
            return this;
        }
        public Embed Build()
        {
            embed.Fields = fields.ToArray();
            return embed;
        }

    }

    public class EmbedFieldBuilder
    {
        private Field embedField = new Field();

        public EmbedFieldBuilder Name(string name)
        {
            embedField.Name = name;
            return this;
        }
        public EmbedFieldBuilder Value(string value)
        {
            embedField.Value = value;
            return this;
        }
        public EmbedFieldBuilder Inline(bool inline)
        {
            embedField.Inline = inline;
            return this;
        }
        public Field Build()
        {
            return embedField;
        }
    }

    public class EmbedAuthorBuilder
    {
        private Author author = new Author();
        
        public EmbedAuthorBuilder Name(string name)
        {
            author.Name = name;
            return this;
        }
        public EmbedAuthorBuilder Url(string url)
        {
            author.Url = url;
            return this;
        }
        public EmbedAuthorBuilder IconUrl(string iconUrl)
        {
            author.IconUrl = iconUrl;
            return this;
        }
        public Author Build()
        {
            return author;
        }
    }

    public class EmbedFooterBuilder
    {
        private Footer footer = new Footer();

        public EmbedFooterBuilder Text(string text)
        {
            footer.Text = text;
            return this;
        }
        public EmbedFooterBuilder IconUrl(string iconUrl)
        {
            footer.IconUrl = iconUrl;
            return this;
        }
        public Footer Build()
        {
            return footer;
        }
    }
}
