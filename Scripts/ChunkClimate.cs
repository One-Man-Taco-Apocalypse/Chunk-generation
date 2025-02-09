using UnityEngine;

public class ChunkClimate : MonoBehaviour
{
    [Header("Noise Settings")]
    [SerializeField] private float temperatureScale = 0.005f; // ✅ Adjusted for proper scaling
    [SerializeField] private float humidityScale = 0.005f;
    [SerializeField] private float foliageScale = 0.005f;

    public void GenerateClimateData(ChunkData chunk, int seed)
    {
        if (chunk == null)
        {
            Debugging.LogError("❌ Chunk is null, cannot generate climate data.");
            return;
        }

        Random.InitState(seed + chunk.ChunkHash); // 🌱 Unique seed per chunk

        float x = chunk.ChunkPosition.x * 0.01f;
        float z = chunk.ChunkPosition.z * 0.01f;

        chunk.Temperature = Mathf.PerlinNoise((x + seed) * temperatureScale, (z + seed) * temperatureScale) * 100f;
        chunk.Humidity = Mathf.PerlinNoise((x + seed + 1000) * humidityScale, (z + seed + 1000) * humidityScale) * 100f;
        chunk.FoliageDensity = Mathf.PerlinNoise((x + seed - 500) * foliageScale, (z + seed - 500) * foliageScale) * 100f;

        Debugging.LogOperation($"🌡️ Climate → {chunk.ChunkName} | Temp: {chunk.Temperature:F1} | Humidity: {chunk.Humidity:F1} | Foliage: {chunk.FoliageDensity:F1}");
    }
}

