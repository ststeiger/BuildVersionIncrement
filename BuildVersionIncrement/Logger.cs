using EnvDTE;
using System;
using System.Diagnostics;
using System.Text;
namespace BuildVersionIncrement
{
	public class Logger
	{
		internal class WriteEventArgs : EventArgs
		{
			public string Message;
			public LogLevel LogLevel;
			public WriteEventArgs(string message, LogLevel logLevel)
			{
				this.Message = message;
				this.LogLevel = logLevel;
			}
		}
		private StringBuilder _contents = new StringBuilder();
		private Connect _connect;
		private static Logger _instance;
		internal static event EventHandler<Logger.WriteEventArgs> WriteEvent;
		internal string Contents
		{
			get
			{
				return this._contents.ToString();
			}
		}
		public static Logger Instance
		{
			get
			{
				return Logger._instance;
			}
		}
		internal Logger(Connect connect)
		{
			this._connect = connect;
			Logger._instance = this;
		}
		public static void Write(string message, LogLevel logLevel)
		{
			Logger.Write(message, logLevel, null, 0);
		}
		public static void Write(string message, LogLevel logLevel, string filename, int line)
		{
			LogLevel logLevel2 = GlobalAddinSettings.Default.IsVerboseLogEnabled ? LogLevel.Debug : LogLevel.Info;
			if (logLevel2 <= logLevel)
			{
				object[] args = new object[]
				{
					logLevel,
					message
				};
				string text = string.Format("[{0}] {1}\r\n", args);
				Logger.Instance._contents.AppendFormat(text, new object[0]);
				if (Logger.WriteEvent != null)
				{
					Logger.WriteEvent(Logger.Instance, new Logger.WriteEventArgs(text, logLevel));
				}
				vsTaskPriority priority;
				vsTaskIcon icon;
				switch (logLevel)
				{
				case LogLevel.Warning:
					priority = vsTaskPriority.vsTaskPriorityMedium;
					icon = vsTaskIcon.vsTaskIconSquiggle;
					break;
				case LogLevel.Error:
					priority = vsTaskPriority.vsTaskPriorityHigh;
					icon = vsTaskIcon.vsTaskIconCompile;
					break;
				default:
				{
					string text2 = string.Format("{0}: {1}\n", "BuildVersionIncrement", message);
					if (!Logger.Instance._connect.IsCommandLineBuild && Logger.Instance._connect.OutputBuildWindow != null)
					{
						Logger.Instance._connect.OutputBuildWindow.OutputString(text2);
					}
					else
					{
						Console.Write(text2);
					}
					Debug.WriteLine(message);
					return;
				}
				}
				if (!Logger.Instance._connect.IsCommandLineBuild && Logger.Instance._connect.OutputBuildWindow != null)
				{
					Logger.Instance._connect.OutputBuildWindow.OutputTaskItemString(message, priority, "BuildVersionIncrement", icon, filename, line, message, true);
					Logger.Instance._connect.ApplicationObject.ToolWindows.ErrorList.Parent.Activate();
				}
				else
				{
					Console.WriteLine("Error: " + message);
					Environment.Exit(1);
				}
			}
		}
	}
}
