using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChunkGenerator))]
public class ChunkGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ChunkGenerator generator = (ChunkGenerator)target;

        if (GUILayout.Button("Regenerate Chunks & Climate"))
        {
            generator.RegenerateChunks();
        }
    }
}
