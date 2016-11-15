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
        private Embed model = new Embed();
        private List<Field> fields = new List<Field>();

        public EmbedBuilder()
        {
            model.Type = "rich";
        }

        public string Title { get { return model.Title; }  set { model.Title = value; } }
        public string Description { get { return model.Description; } set { model.Description = value; } }
        public string Url { get { return model.Url; } set { model.Url = value; } }
        public Color? Color { get { return model.Color.HasValue ? new Color(model.Color.Value) : (Color?)null; } set { model.Color = value?.RawValue; } }

        public void SetAuthor(Action<EmbedBuilderAuthor> action)
        {
            var author = new EmbedBuilderAuthor();
            action(author);
            model.Author = author.ToModel();
        }
        public void SetFooter(Action<EmbedBuilderFooter> action)
        {
            var footer = new EmbedBuilderFooter();
            action(footer);
            model.Footer = footer.ToModel();
        }
        public void AddField(Action<EmbedBuilderField> action)
        {
            var field = new EmbedBuilderField();
            action(field);
            fields.Add(field.ToModel());
        }

        internal Embed Build()
        {
            model.Fields = fields.ToArray();
            return model;
        }
    }

    public class EmbedBuilderField
    {
        private Field model = new Field();

        public string Name { get { return model.Name; } set { model.Name = value; } }
        public string Value { get { return model.Value; } set { model.Value = value; } }
        public bool IsInline { get { return model.Inline; } set { model.Inline = value; } }

        internal Field ToModel() => model;
    }

    public class EmbedBuilderAuthor
    {
        private Author model = new Author();

        public string Name { get { return model.Name; } set { model.Name = value; } }
        public string Url { get { return model.Url; } set { model.Url = value; } }
        public string IconUrl { get { return model.IconUrl; } set { model.IconUrl = value; } }

        internal Author ToModel() => model;
    }

    public class EmbedBuilderFooter
    {
        private Footer model = new Footer();

        public string Text { get { return model.Text; } set { model.Text = value; } }
        public string IconUrl { get { return model.IconUrl; } set { model.IconUrl = value; } }

        internal Footer ToModel() => model;
    }
}
