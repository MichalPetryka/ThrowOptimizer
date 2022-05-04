using System;
using Microsoft.Build.Framework;

namespace ThrowOptimizer.Tasks
{
	internal class LogHandler : Utils.LogHandler.ILogHandler
	{
		private readonly ThrowTask _throwTask;

		public LogHandler(ThrowTask throwTask)
		{
			_throwTask = throwTask;
		}

		public void Log(string message, Utils.LogHandler.LogType logType)
		{
			switch (logType)
			{
				case Utils.LogHandler.LogType.Debug:
					_throwTask.Log.LogMessage(MessageImportance.Low, message);
					break;
				case Utils.LogHandler.LogType.Info:
					_throwTask.Log.LogMessage(MessageImportance.Normal, message);
					break;
				case Utils.LogHandler.LogType.Warn:
					_throwTask.Log.LogWarning(message);
					break;
				case Utils.LogHandler.LogType.Error:
					_throwTask.Log.LogError(message);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
			}
		}
	}
}