using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChunkGizmo : MonoBehaviour
{
    private ChunkGenerator chunkGenerator;

    private void Awake()
    {
        chunkGenerator = FindObjectOfType<ChunkGenerator>();
    }

    private void OnDrawGizmos()
    {
        if (chunkGenerator == null || chunkGenerator.GetChunks().Count == 0)
        {
            Debugging.LogWarning("🔍 No chunks available, skipping Gizmos.");
            return;
        }

        foreach (ChunkData chunk in chunkGenerator.GetChunks())
        {
            Vector3 center = chunk.ChunkPosition + new Vector3(chunkGenerator.GetChunkSize() / 2, 0, chunkGenerator.GetChunkSize() / 2);

            if (Debugging.ShowChunkOutlines)
            {
                Gizmos.color = Debugging.GetChunkOutlineColor();
                DrawChunkOutline(chunk.ChunkPosition, chunkGenerator.GetChunkSize());
            }

            if (Debugging.ShowChunkNames)
            {
                DrawChunkName(chunk.ChunkName, center);
            }
        }
    }

    private void DrawChunkOutline(Vector3 position, float size)
    {
        Vector3[] points = {
            position, position + new Vector3(size, 0, 0),
            position + new Vector3(size, 0, size), position + new Vector3(0, 0, size)
        };

        for (int i = 0; i < points.Length; i++)
            Gizmos.DrawLine(points[i], points[(i + 1) % points.Length]);
    }

    private void DrawChunkName(string name, Vector3 position)
    {
#if UNITY_EDITOR
        GUIStyle style = new GUIStyle
        {
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Debugging.GetChunkNameColor() },
            fontSize = 12
        };
        Handles.Label(position, name, style);
#endif
    }
}
