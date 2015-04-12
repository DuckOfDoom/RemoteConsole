using System.Collections;
using UnityEngine;

namespace RemoteConsole
{
	/// <summary>
	/// 
	/// </summary>
	public class RemoteConsoleBehaviour : MonoBehaviour
	{
		[SerializeField] private string _url;
		private readonly RemoteConsole _console = new RemoteConsole();

		/// <summary>
		/// Log level filter
		/// </summary>
		public LogLevel LogLevel { get { return _console.LogLevel; } set { _console.LogLevel = value; } }

		/// <summary>
		/// URL to send requests to
		/// </summary>
		public string URL
		{
			get { return _url; }
			set
			{
				_url = value;
				_console.URL = value;
			}
		}

		public void Awake()
		{
			DontDestroyOnLoad(gameObject);
			Application.RegisterLogCallback(_console.HandleLog);
			StartCoroutine(SendLogs());
		}

		private IEnumerator SendLogs()
		{
			while (true)
			{
				var requestString = _console.GetNextLogRequest();
				if (!string.IsNullOrEmpty(requestString))
				{
					var www = new WWW(requestString);

					yield return www;

					if (string.IsNullOrEmpty(www.error))
						_console.OnLogSent();
					else
						Debug.LogError("ERROR: " + www.error);
				}

				yield return null;
			}
		}
	}
}
