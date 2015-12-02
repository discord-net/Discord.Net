using System.Threading.Tasks;

namespace Discord
{
	public static class TaskHelper
	{
		public static Task CompletedTask { get; }
		static TaskHelper()
		{
#if DOTNET5_4
			CompletedTask = Task.CompletedTask;
#else
			CompletedTask = Task.Delay(0);
#endif
		}
	}
}
