using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerScoreChangeUI))]
public class ChangeScoreUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerScoreChangeUI playerScoreChangeUI = target as PlayerScoreChangeUI;

        EditorGUILayout.LabelField("QA Testing");
        if (GUILayout.Button("Animate from start "))
        {
            if (playerScoreChangeUI != null) playerScoreChangeUI.Animate();
        }

    }
}