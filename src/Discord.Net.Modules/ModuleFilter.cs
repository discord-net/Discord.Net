using System;

namespace Discord.Modules
{
	[Flags]
    public enum ModuleFilter
    {
		/// <summary> Disables the event and command filters. </summary>
		None = 0x0,
		/// <summary> Uses the server whitelist to filter events and commands. </summary>
		ServerWhitelist = 0x1,
		/// <summary> Uses the channel whitelist to filter events and commands.  </summary>
		ChannelWhitelist = 0x2,
		/// <summary> Enables this module in all private messages.  </summary>
		AlwaysAllowPrivate = 0x4
    }
}
