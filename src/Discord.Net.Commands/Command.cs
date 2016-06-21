using System;
using System.Reflection;

namespace Discord.Commands
{
    public class Command
    {
        private Action<IMessage> _action;

        public string Name { get; }
        public string Description { get; }
        public string Text { get; }

        internal Command(CommandAttribute attribute, MethodInfo methodInfo)
        {
            var description = methodInfo.GetCustomAttribute<DescriptionAttribute>();
            if (description != null)
                Description = description.Text;

            Name = attribute.Name;
            Text = attribute.Text;
        }

        public void Invoke(IMessage msg)
        {
            _action.Invoke(msg);
        }

        private void BuildAction()
        {
            _action = null;
            //TODO: Implement
        }
    }
}
