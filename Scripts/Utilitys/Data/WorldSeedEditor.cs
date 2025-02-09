using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldSeed))]
public class WorldSeedEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector (shows the current seed).
        DrawDefaultInspector();

        WorldSeed ws = (WorldSeed)target;
        EditorGUILayout.Space();

        if (GUILayout.Button("Regenerate Seed"))
        {
            ws.RegenerateSeed();
        }
    }
}
