using System;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
	private void OnGUI()
	{
		var width = GUILayout.Width(Screen.width / 2f);
		var height = GUILayout.Height(Screen.height / 5f);

		//_behaviour.URL = GUILayout.TextField(_behaviour.URL, width, height);

		if (GUILayout.Button("LOG", width, height))
			Debug.Log("Log");

		if (GUILayout.Button("Warning", width, height))
			Debug.LogWarning("Warning...");

		if (GUILayout.Button("Error", width, height))
			Debug.LogError("Error!");

		if (GUILayout.Button("EXCEPTION", width, height))
			Debug.LogException(new ArgumentOutOfRangeException("YOUR MOM"));
	}
}
