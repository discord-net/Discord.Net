using System;
namespace Discord.Gateway
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ETFNameAttribute : Attribute
	{
		public string Name
			=> _name;

		private readonly string _name;

		public ETFNameAttribute(string name)
		{
			_name = name;
		}
	}
}

