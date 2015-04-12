using System;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteConsole
{
	public class RemoteConsole
	{
		private string _url = "127.0.0.1:8080";

		private const string Separator = "|";
		private const string LogURIPrefix = "?log=";

		private readonly List<string> _logs = new List<string>();

		public LogLevel LogLevel { get; set; }
		public string URL { get { return _url; } set { _url = value; } }

		public string GetNextLogRequest()
		{
			if (_logs.Count <= 0)
				return null;

			return _url + LogURIPrefix + _logs[_logs.Count - 1];
		}

		/// <summary>
		///		A callback to handle unity logs
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="stackTrace"></param>
		/// <param name="type"></param>
		public void HandleLog(string condition, string stackTrace, LogType type)
		{
			Debug.Log(string.Format("C: {0}, st: {1}, type:{2}", condition, stackTrace, type));

			switch (type)
			{
				case LogType.Log:
					if (LogLevel == LogLevel.All)
						QueueLog(condition, stackTrace);
					break;
				case LogType.Warning:
					if (LogLevel == LogLevel.All || LogLevel == LogLevel.WarningsAndErrors)
						QueueLog(condition, stackTrace);
					break;
				case LogType.Assert:
				case LogType.Exception:
				case LogType.Error:
					QueueLog(condition, stackTrace);
					break;
				default:
					throw new ArgumentOutOfRangeException("type");
			}
		}

		/// <summary>
		///		Callback that should be called when previous log piece was sent successfully
		/// </summary>
		public void OnLogSent()
		{
			_logs.RemoveAt(_logs.Count - 1);
		}

		private void QueueLog(string condition, string stackTrace)
		{
			_logs.Add(condition + Separator + stackTrace);
		}
	}

	public enum LogLevel
	{
		All = 0,
		WarningsAndErrors = 1,
		ErrorsOnly = 2
	}
}
