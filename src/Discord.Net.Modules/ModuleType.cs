using System;

namespace Discord.Modules
{
	[Flags]
    public enum FilterType
    {
		/// <summary> Disables the event filter. </summary>
		Disabled = 0x0,
		/// <summary> Uses the server whitelist to filter events. </summary>
		Server = 0x1,
		/// <summary> Uses the channel whitelist to filter events.  </summary>
		Channel = 0x2,
		/// <summary> Enables this module in all private messages.  </summary>
		AllowPrivate = 0x4
    }
}
