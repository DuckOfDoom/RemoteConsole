using UnityEditor;

namespace RemoteConsole
{
	[CustomEditor(typeof (RemoteConsoleBehaviour))]
	public class RemoteConsoleBehaviourEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var behaviour = (RemoteConsoleBehaviour) target;

			behaviour.URL = EditorGUILayout.TextField("URL: ", behaviour.URL);
			behaviour.LogLevel = (LogLevel)EditorGUILayout.EnumPopup("Log Level: ", behaviour.LogLevel);
		}
	}
}
