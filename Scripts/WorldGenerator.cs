using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

[ExecuteAlways]
public class WorldGenerator : MonoBehaviour
{
    [Header("Biome Settings")]
    [SerializeField] private List<Biome> biomes = new List<Biome>();

    private ChunkGenerator chunkGenerator;
    private ChunkClimate chunkClimate;
    private WorldSeed worldSeedComponent;

    // Reusable temporary collections for biome smoothing.
    private readonly List<ChunkData> neighborList = new List<ChunkData>();
    private readonly Dictionary<Biome, int> localBiomeCounts = new Dictionary<Biome, int>();

    private void Awake()
    {
        chunkGenerator = FindObjectOfType<ChunkGenerator>();
        chunkClimate = FindObjectOfType<ChunkClimate>();
        worldSeedComponent = FindObjectOfType<WorldSeed>();

        if (chunkGenerator == null)
        {
            Debug.LogError("❌ ChunkGenerator not found!");
            return;
        }
        if (chunkClimate == null)
        {
            Debug.LogWarning("⚠️ ChunkClimate not found! Adding one to this GameObject.");
            chunkClimate = gameObject.AddComponent<ChunkClimate>();
        }

        Debug.Log("✅ World Generator Initialized.");
    }

    /// <summary>
    /// Regenerates the world by:
    /// 1. Regenerating the world seed (if a WorldSeed component is found),
    /// 2. Regenerating chunks,
    /// 3. Generating climate data, and then
    /// 4. Smoothing biome transitions.
    /// </summary>
    public void RegenerateWorld()
    {
        Debug.Log("🔄 Regenerating World...");

        // 1. Regenerate the world seed.
        if (worldSeedComponent != null)
        {
            worldSeedComponent.RegenerateSeed();
            Debug.Log("🌍 New world seed: " + worldSeedComponent.Seed);
        }
        else
        {
            Debug.LogWarning("⚠️ WorldSeed component not found. No seed regeneration will occur.");
        }

        // 2. Regenerate chunks.
        chunkGenerator.RegenerateChunks();

        // If in Play mode, wait until chunk generation is complete.
        if (Application.isPlaying)
        {
            StartCoroutine(WaitForChunkGenerationThenUpdate());
        }
        else
        {
            // In Editor mode, generation is synchronous.
            GenerateClimateData();
            SmoothBiomes();
        }
    }

    /// <summary>
    /// Waits until chunk generation is complete, then generates climate data and smooths biomes.
    /// </summary>
    private IEnumerator WaitForChunkGenerationThenUpdate()
    {
        // Wait until chunk generation is finished.
        while (chunkGenerator.IsGenerating)
        {
            yield return null;
        }
        GenerateClimateData();
        SmoothBiomes();
    }

    /// <summary>
    /// Generates climate data on all chunks and assigns an initial biome based on thresholds.
    /// </summary>
    public void GenerateClimateData()
    {
        var chunks = chunkGenerator.GetChunks();
        if (chunks == null || chunks.Count == 0)
        {
            Debug.LogError("❌ No chunks available to generate climate data.");
            return;
        }

        Debug.Log("🌡️ Generating Climate Data for Chunks...");
        foreach (ChunkData chunk in chunks)
        {
            // Generate climate values (this uses the world seed in ChunkClimate).
            chunkClimate.GenerateClimateData(chunk);
            // Assign an initial biome based on climate thresholds and spawn probability.
            AssignBiome(chunk);
        }
        Debug.Log("✅ Initial Climate & Biome Assignment Complete!");
    }

    /// <summary>
    /// Assigns a biome to a chunk based on temperature, humidity, foliage, and spawn weight.
    /// </summary>
    private void AssignBiome(ChunkData chunk)
    {
        if (biomes == null || biomes.Count == 0)
        {
            Debug.LogWarning("⚠️ No biomes available for assignment.");
            chunk.AssignedBiome = null;
            return;
        }

        Biome bestBiome = null;
        float bestWeightedError = float.MaxValue;

        // Loop through all biomes to compute a suitability error.
        foreach (Biome biome in biomes)
        {
            float error = 0f;

            // Temperature error:
            if (chunk.Temperature < biome.temperatureRange.x)
                error += (biome.temperatureRange.x - chunk.Temperature);
            else if (chunk.Temperature > biome.temperatureRange.y)
                error += (chunk.Temperature - biome.temperatureRange.y);

            // Humidity error:
            if (chunk.Humidity < biome.humidityRange.x)
                error += (biome.humidityRange.x - chunk.Humidity);
            else if (chunk.Humidity > biome.humidityRange.y)
                error += (chunk.Humidity - biome.humidityRange.y);

            // Foliage error:
            if (chunk.FoliageDensity < biome.foliageRange.x)
                error += (biome.foliageRange.x - chunk.FoliageDensity);
            else if (chunk.FoliageDensity > biome.foliageRange.y)
                error += (chunk.FoliageDensity - biome.foliageRange.y);

            // If error is 0, it's a perfect match; choose it immediately.
            if (error == 0f)
            {
                bestBiome = biome;
                bestWeightedError = 0f;
                break;
            }

            // Compute weighted error: lower values are better.
            float weightedError = error / biome.spawnWeight;
            if (weightedError < bestWeightedError)
            {
                bestWeightedError = weightedError;
                bestBiome = biome;
            }
        }

        if (bestBiome != null)
        {
            chunk.AssignedBiome = bestBiome;
            Debug.Log("🌍 Assigned Biome: " + bestBiome.biomeName + " to " + chunk.ChunkName + " (Weighted Error: " + bestWeightedError + ")");
        }
        else
        {
            Debug.LogWarning("⚠️ No suitable biome found for " + chunk.ChunkName + ". No biome assigned.");
            chunk.AssignedBiome = null;
        }
    }

    /// <summary>
    /// Smooths biome transitions by reassigning each chunk’s biome based on the majority biome among its immediate neighbors.
    /// </summary>
    private void SmoothBiomes()
    {
        var chunks = chunkGenerator.GetChunks();
        if (chunks == null || chunks.Count == 0)
        {
            Debug.LogError("❌ No chunks available for biome smoothing.");
            return;
        }

        // For each chunk, find its neighbors and count their biome frequencies.
        foreach (ChunkData chunk in chunks)
        {
            neighborList.Clear();
            // Collect neighbors (within 1 grid cell in each direction, including diagonals).
            foreach (ChunkData other in chunks)
            {
                if (other == chunk)
                    continue;
                if (Mathf.Abs(other.XIndex - chunk.XIndex) <= 1 &&
                    Mathf.Abs(other.ZIndex - chunk.ZIndex) <= 1)
                {
                    neighborList.Add(other);
                }
            }

            localBiomeCounts.Clear();
            // Count frequencies of each biome among neighbors.
            foreach (ChunkData neighbor in neighborList)
            {
                if (neighbor.AssignedBiome == null)
                    continue;
                if (!localBiomeCounts.ContainsKey(neighbor.AssignedBiome))
                    localBiomeCounts[neighbor.AssignedBiome] = 0;
                localBiomeCounts[neighbor.AssignedBiome]++;
            }

            // Determine the majority biome among neighbors.
            Biome majorityBiome = chunk.AssignedBiome; // default to current biome.
            int maxCount = 0;
            foreach (var kvp in localBiomeCounts)
            {
                if (kvp.Value > maxCount)
                {
                    maxCount = kvp.Value;
                    majorityBiome = kvp.Key;
                }
            }
            chunk.AssignedBiome = majorityBiome;
        }

        Debug.Log("✅ Biome smoothing complete.");
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            // Delay the call so that object creation/destruction is safe in Editor mode.
            EditorApplication.delayCall += () =>
            {
                if (this != null)
                {
                    RegenerateWorld();
                }
            };
        }
    }
#endif
}
