using System.Threading.Tasks;

namespace Discord
{
	internal static class TaskHelper
	{
		public static Task CompletedTask { get; }
		static TaskHelper()
		{
#if DNXCORE50
			CompletedTask = Task.CompletedTask;
#else
			CompletedTask = Task.Delay(0);
#endif
		}
	}
}
