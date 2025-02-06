#define Codecanyon


using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(dotmobStudio))]
public class dotmobStudioInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Edit Game Settings (Admob, In-app Purchase..)", EditorStyles.boldLabel);

        if (GUILayout.Button("Edit Game Settings", GUILayout.MinHeight(40)))
        {
#if Codecanyon
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Sidetheball/Common/Prefabs/GameMaster.prefab");
#else
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Common/Prefabs/GameMaster.prefab");
#endif
        }
		
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Contact Us For Best Support", EditorStyles.boldLabel);
        EditorGUILayout.TextField("Email: ", "dotmobstudio@gmail.com");
    }
}