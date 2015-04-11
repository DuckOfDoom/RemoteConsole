using System;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
	private void OnGUI()
	{
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
