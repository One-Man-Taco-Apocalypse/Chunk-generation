using UnityEngine;
using UnityEditor;

public class ClimateGizmo : MonoBehaviour
{
    public enum ClimateType { Temperature, Humidity, Foliage, Biome }

    [Header("Gizmo Settings")]
    [SerializeField] private ClimateType climateDisplay = ClimateType.Biome; // ✅ Allows switching visualization
    [SerializeField] private bool showClimateGizmos = true;
    [SerializeField] private float gizmoAlpha = 0.5f;
    
    private ChunkGenerator chunkGenerator;

    private void Awake()
    {
        chunkGenerator = FindObjectOfType<ChunkGenerator>();

        if (chunkGenerator == null)
        {
            Debugging.LogError("❌ ClimateGizmo could not find ChunkGenerator!");
            enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showClimateGizmos || chunkGenerator == null) return;

        var chunks = chunkGenerator.GetChunks();
        if (chunks == null || chunks.Count == 0)
        {
            Debugging.LogWarning("⚠️ No chunks found. Skipping Climate Gizmo.");
            return;
        }

        Debugging.LogOperation($"🎨 Drawing {climateDisplay} Climate Gizmos...");

        foreach (var chunk in chunks)
        {
            if (chunk == null) continue;

            Color gizmoColor = GetGizmoColor(chunk);
            gizmoColor.a = gizmoAlpha; // Apply transparency

            Vector3 center = chunk.ChunkPosition + new Vector3(chunkGenerator.GetChunkSize() / 2, 0, chunkGenerator.GetChunkSize() / 2);
            Vector3 size = new Vector3(chunkGenerator.GetChunkSize(), 0.1f, chunkGenerator.GetChunkSize());

            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(center, size);
        }
    }

    private Color GetGizmoColor(ChunkData chunk)
    {
        switch (climateDisplay)
        {
            case ClimateType.Temperature:
                return Color.Lerp(Color.blue, Color.red, chunk.Temperature / 100f); // Cold → Hot
            case ClimateType.Humidity:
                return Color.Lerp(Color.yellow, Color.blue, chunk.Humidity / 100f); // Dry → Wet
            case ClimateType.Foliage:
                return Color.Lerp(new Color(0.6f, 0.3f, 0.1f), Color.green, chunk.FoliageDensity / 100f); // Barren → Lush
            case ClimateType.Biome:
                return chunk.AssignedBiome != null ? chunk.AssignedBiome.biomeColor : Color.white;
            default:
                return Color.white;
        }
    }
}
