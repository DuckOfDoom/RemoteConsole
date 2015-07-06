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
	///     Remote Console allows sending Unity3D logs via POST HTTP requests to remote server.
	///     USAGE: Add this to any GameObject and fill "Server Address" field with IP of the server that will accept POST
	///     request on "/logs" URL.
	/// </summary>
	public class RemoteConsole : MonoBehaviour
	{
#pragma warning disable 649

		/// <summary>
		///     Log Level filtering to send requests for.
		/// </summary>
		[SerializeField] private LogLevel _logLevel = LogLevel.ErrorsOnly;

		/// <summary>
		///     Whether to report connection errors or not.
		///     Useful if you're not sure if your server will be up the whole time and want to avoid the spam in logs.
		/// </summary>
		[SerializeField] private bool _reportConnectionErrors;

		/// <summary>
		///		Whether to send data when runing from UnityEditor.
		/// </summary>
		[SerializeField] private bool _sendFromEditor;

		/// <summary>
		///     IP Address of the server to send requests to, i.e. "192.168.0.100:3000".
		/// </summary>
		[SerializeField] private string _serverAddress = "localhost:3000";

#pragma warning restore 649

		private const string ServerPostUrl = "/logs";
		private readonly Queue<string> _logs = new Queue<string>();

		public void OnDestroy()
		{
#if UNITY_5
			Application.logMessageReceived -= HandleLog;
#else
			Application.RegisterLogCallback(null);
#endif
		}

		public void Awake()
		{
			DontDestroyOnLoad(gameObject);

			// NOTE: On Unity Version < 5.0.0 you can't have multiple LogCallback handlers.
			// It means that if you have some other callbacks registered in project before this script, 
			// they will be unregistered for the sake of this one and vice versa.

#if UNITY_5
			Application.logMessageReceived += HandleLog;
#else
			Application.RegisterLogCallback(HandleLog);
#endif
			StartCoroutine(SendRequestsCoroutine());
		}

		/// <summary>
		///     Build ID. Can be overriden for custom builds.
		/// </summary>
		protected virtual string BuildID { get { return "Not Specified"; } }

		/// <summary>
		///     Device ID. Can be overriden for custom device IDs
		/// </summary>
		protected virtual string DeviceID { get { return SystemInfo.deviceModel + "/" + SystemInfo.deviceName; } }

		/// <summary>
		///     A callback to handle unity logs
		/// </summary>
		private void HandleLog(string log, string stackTrace, LogType type)
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

		private IEnumerator SendRequestsCoroutine()
		{
			while (true)
			{	
				if (!_sendFromEditor && Application.isEditor)
					yield return null;

				if (_logs.Any())
				{
					var log = _logs.Peek();
					var www = new WWW("http://" + _serverAddress + ServerPostUrl,
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

		/// <summary>
		///     Levels to filter messages
		/// </summary>
		public enum LogLevel
		{
			All,
			WarningsAndErrors,
			ErrorsOnly
		}
	}
}

