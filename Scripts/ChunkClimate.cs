using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class ChunkClimate : MonoBehaviour
{
    [Header("Noise Settings")]
    [SerializeField] private float temperatureScale = 5f; // Adjust as needed
    [SerializeField] private float humidityScale = 5f;
    [SerializeField] private float foliageScale = 5f;

    // Cache reference to the ChunkGenerator to obtain world size.
    private ChunkGenerator chunkGenerator;
    // Cache reference to WorldSeed for performance.
    private WorldSeed worldSeedComponent;

    private void OnEnable()
    {
        if (chunkGenerator == null)
            chunkGenerator = FindObjectOfType<ChunkGenerator>();
        if (worldSeedComponent == null)
            worldSeedComponent = FindObjectOfType<WorldSeed>();
    }

    /// <summary>
    /// Generates climate data (temperature, humidity, foliage density) for a given chunk.
    /// Uses normalized chunk coordinates plus separate seed offsets for each climate type.
    /// </summary>
    public void GenerateClimateData(ChunkData chunk)
    {
        if (chunk == null)
        {
            Debug.LogError("❌ Chunk is null; cannot generate climate data.");
            return;
        }
        
        if (chunkGenerator == null)
        {
            chunkGenerator = FindObjectOfType<ChunkGenerator>();
            if (chunkGenerator == null)
            {
                Debug.LogError("❌ ChunkGenerator not found for climate generation.");
                return;
            }
        }
        
        // Normalize the chunk's grid indices to a 0–1 range.
        int worldSize = chunkGenerator.WorldSizeInChunks;
        float normalizedX = (chunk.XIndex + (worldSize / 2f)) / worldSize;
        float normalizedZ = (chunk.ZIndex + (worldSize / 2f)) / worldSize;
        
        // Get separate offsets for each climate type from the WorldSeed component.
        float offsetTemp = 0f;
        float offsetHumidity = 0f;
        float offsetFoliage = 0f;
        if (worldSeedComponent != null)
        {
            // Derive three different offsets from the seed.
            offsetTemp = Mathf.Abs(worldSeedComponent.Seed % 1000) / 1000f;
            offsetHumidity = Mathf.Abs((worldSeedComponent.Seed / 1000) % 1000) / 1000f;
            offsetFoliage = Mathf.Abs((worldSeedComponent.Seed / 1000000) % 1000) / 1000f;
        }
        
        // Temperature: use its own offset.
        float tempInputX = (normalizedX + offsetTemp) * temperatureScale;
        float tempInputZ = (normalizedZ + offsetTemp) * temperatureScale;
        chunk.Temperature = Mathf.PerlinNoise(tempInputX, tempInputZ) * 100f;
        
        // Humidity: use a different offset.
        float humInputX = (normalizedX + offsetHumidity + 1000f) * humidityScale;
        float humInputZ = (normalizedZ + offsetHumidity + 1000f) * humidityScale;
        chunk.Humidity = Mathf.PerlinNoise(humInputX, humInputZ) * 100f;
        
        // Foliage Density: use yet another offset.
        float folInputX = (normalizedX + offsetFoliage - 500f) * foliageScale;
        float folInputZ = (normalizedZ + offsetFoliage - 500f) * foliageScale;
        chunk.FoliageDensity = Mathf.PerlinNoise(folInputX, folInputZ) * 100f;

        // (Optional) Log the computed climate values:
        // Debug.Log($"🌡️ {chunk.ChunkName}: Temp={chunk.Temperature:F1}, Humidity={chunk.Humidity:F1}, Foliage={chunk.FoliageDensity:F1}");
    }
}
