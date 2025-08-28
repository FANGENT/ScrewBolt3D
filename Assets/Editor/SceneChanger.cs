using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SceneChanger : EditorWindow {

	// Add menu named "My Window" to the Window menu
	[MenuItem ("Asif Mushtaq/Scene Changer")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		SceneChanger window = (SceneChanger)EditorWindow.GetWindow (typeof (SceneChanger));
		window.Show();
	}

	void OnGUI () {

		//GUILayout.Label ("Scene Changer", EditorStyles.boldFont);
		EditorGUILayout.LabelField("Scene Changer", EditorStyles.boldLabel);

		if (EditorApplication.isCompiling) {
			
			EditorGUILayout.BeginVertical ("Box");
			EditorGUILayout.LabelField ("Compiling Scripts");
			EditorGUILayout.EndVertical ();

		} else if (EditorApplication.isPlayingOrWillChangePlaymode) {
			
			EditorGUILayout.BeginVertical ("Box");
			EditorGUILayout.LabelField ("Play Mode Active");
			EditorGUILayout.EndVertical ();

		} else if (EditorApplication.isUpdating) {
			
			EditorGUILayout.BeginVertical ("Box");
			EditorGUILayout.LabelField ("Updating AssetDatabase");
			EditorGUILayout.EndVertical ();

		} else {

			EditorGUILayout.Space ();
			EditorGUILayout.BeginHorizontal ("Box");
			if (GUILayout.Button ("New Scene")) {
				UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
			}
			if (GUILayout.Button ("Save Scene")) {
				UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes ();
			}
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Space ();

			foreach (UnityEditor.EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes) {

				bool isOpenScene = (scene.path == UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ().path);

				//EditorGUILayout.BeginVertical ( isOpenScene ? "Box" : EditorStyles.helpBox);
				EditorGUILayout.BeginVertical ( "Box");

				string[] s1 = scene.path.Split (new string[] { "/" }, System.StringSplitOptions.None);
				string s2 = s1[s1.Length - 1];
				string[] s3 = s2.Split (new string[] { "." }, System.StringSplitOptions.None);
				string sceneName = s3[0];

				EditorGUILayout.LabelField(sceneName, isOpenScene ? EditorStyles.boldLabel : EditorStyles.inspectorDefaultMargins);
				//EditorGUILayout.LabelField (sceneName, EditorStyles.boldFont );


				if (GUILayout.Button (isOpenScene ? "Save and Reload" : "Open")) {
					
					UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes ();
					UnityEditor.SceneManagement.EditorSceneManager.OpenScene (scene.path);

				}

				EditorGUILayout.EndVertical ();
			}


		}

		if (EditorApplication.isRemoteConnected) {
			EditorGUILayout.BeginVertical ("Box");
			EditorGUILayout.LabelField ("Unity Remove Connected");
			EditorGUILayout.EndVertical ();
		}
//
//		EditorGUILayout.Space ();
//		EditorGUILayout.BeginVertical ("Box");
//		EditorGUILayout.LabelField ("" + heldSelection);
//		if (GUILayout.Button ("Hold Selection")) {
//			heldSelection = Selection.activeInstanceID;//Selection.activeGameObject;
//		}
//		if (heldSelection != null) {
//			if (GUILayout.Button ("Reselect")) {
//				Selection.activeGameObject = EditorUtility.InstanceIDToObject (heldSelection) as GameObject;//heldSelection;
//			}
//		}
//		EditorGUILayout.EndVertical();
//
//		if (Selection.activeGameObject != null)
//			EditorGUILayout.LabelField ("" + Selection.activeInstanceID);
	}

}
