using System;
using UnityEngine;

namespace RemoteConsole
{
	public class RemoteConsoleTestBehaviour : MonoBehaviour
	{
		private void OnGUI()
		{
			var width = GUILayout.Width(Screen.width / 2f);
			var height = GUILayout.Height(Screen.height / 5f);

			if (GUILayout.Button("LOG", width, height))
				Debug.Log("This is just a test log...");

			if (GUILayout.Button("Warning", width, height))
				Debug.LogWarning("This is a test warning...");

			if (GUILayout.Button("Error", width, height))
				Debug.LogError("This is a teset error!");

			if (GUILayout.Button("EXCEPTION", width, height))
				Debug.LogException(new Exception("This is a test exception!"));
		}
	}
}
