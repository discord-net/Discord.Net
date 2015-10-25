using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
	internal static class BitHelper
	{
		public static bool GetBit(uint value, int pos) => ((value >> (byte)pos) & 1U) == 1;
		public static void SetBit(ref uint value, int pos, bool bitValue)
		{
			if (bitValue)
				value |= (1U << pos);
			else
				value &= ~(1U << pos);
		}
	}
}
