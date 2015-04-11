using System;
using RemoteConsole;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
	private RemoteConsoleBehaviour _behaviour;

	private void Awake()
	{
		_behaviour = FindObjectOfType<RemoteConsoleBehaviour>();
	}

	private void OnGUI()
	{
		_behaviour.URL = GUILayout.TextField(_behaviour.URL);

		if (GUILayout.Button("LOG"))
			Debug.Log("Log");

		if (GUILayout.Button("Warning"))
			Debug.LogWarning("Warning...");

		if (GUILayout.Button("Error"))
			Debug.LogError("Error!");

		if (GUILayout.Button("EXCEPTION"))
			Debug.LogException(new ArgumentOutOfRangeException("YOUR MOM"));
	}
}
