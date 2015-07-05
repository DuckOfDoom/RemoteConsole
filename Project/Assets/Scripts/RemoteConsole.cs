using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RemoteConsole.MiniJSON;
using UnityEngine;

namespace RemoteConsole
{
	/// <summary>
	/// 
	/// </summary>
	public class RemoteConsole : MonoBehaviour
	{
#pragma warning disable 649
		[SerializeField] private string _buildID;
		[SerializeField] private LogLevel _logLevel;
		[SerializeField] private string _serverAddress;
		[SerializeField] private bool _reportConnectionErrors;
#pragma warning restore 649

		private const string ServerPostURL = "/logs";
		private readonly Queue<string> _logs = new Queue<string>();

		public void Awake()
		{
			DontDestroyOnLoad(gameObject);
			Application.RegisterLogCallback(HandleLog);
			StartCoroutine(SendLogs());
		}

		/// <summary>
		///		A callback to handle unity logs
		/// </summary>
		/// <param name="log"></param>
		/// <param name="stackTrace"></param>
		/// <param name="type"></param>
		public void HandleLog(string log, string stackTrace, LogType type)
		{
			switch (type)
			{
				case LogType.Log:
					if (_logLevel == LogLevel.All)
						QueueLog(type, log, stackTrace);
					break;
				case LogType.Warning:
					if (_logLevel == LogLevel.All || _logLevel == LogLevel.WarningsAndErrors)
						QueueLog(type, log, stackTrace);
					break;
				case LogType.Assert:
				case LogType.Exception:
				case LogType.Error:
					QueueLog(type, log, stackTrace);
					break;
				default:
					throw new ArgumentOutOfRangeException("type");
			}
		}

		protected virtual string BuildID { get { return _buildID; } }
		protected virtual string DeviceID { get { return SystemInfo.deviceModel + "/" + SystemInfo.deviceName; } }

		private void QueueLog(LogType logType, string log, string stackTrace)
		{
			var timestamp = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
			var dict = new Dictionary<string, object>
			{
				{"device_id", DeviceID},
				{"build_id", BuildID},
				{"log_type", logType},
				{"log", log},
				{"stack_trace", stackTrace},
				{"timestamp", timestamp}
			};
			_logs.Enqueue(Json.Serialize(dict));
		}

		private IEnumerator SendLogs()
		{
			while (true)
			{
				if (_logs.Any())
				{
					var log = _logs.Peek();
					var www = new WWW("http://" + _serverAddress + ServerPostURL,
					                  Encoding.UTF8.GetBytes(log),
					                  new Dictionary<string, string>
					                  {
						                  {"Content-Type", "application/json"}
					                  });

					yield return www;

					if (string.IsNullOrEmpty(www.error))
						_logs.Dequeue();
					else if (_reportConnectionErrors)
						Debug.LogError("Failed to send request. Error: " + www.error);
				}

				yield return null;
			}
		}

		public enum LogLevel
		{
			All,
			WarningsAndErrors,
			ErrorsOnly
		}
	}
}
