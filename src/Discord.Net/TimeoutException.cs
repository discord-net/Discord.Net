using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
	public sealed class TimeoutException : Exception
	{
		internal TimeoutException()
			: base("An operation has timed out.")
		{
		}
	}
}
