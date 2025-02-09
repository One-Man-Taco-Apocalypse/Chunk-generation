using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChunkGenerator))]
public class ChunkGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ChunkGenerator generator = (ChunkGenerator)target;

        if (GUILayout.Button("Regenerate Chunks & Climate"))
        {
            // Preferably, call the WorldGenerator to update both chunks and climate.
            WorldGenerator wg = FindObjectOfType<WorldGenerator>();
            if (wg != null)
            {
                wg.RegenerateWorld();
            }
            else
            {
                generator.RegenerateChunks();
            }
        }
    }
}
