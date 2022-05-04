namespace ThrowOptimizer.Utils
{
	public static class LogHandler
	{
		public enum LogType
		{
			Debug,
			Info,
			Warn,
			Error
		}

		public interface ILogHandler
		{
			void Log(string message, LogType logType);
		}
	}
}
