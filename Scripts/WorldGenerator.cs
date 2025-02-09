using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGenerator : MonoBehaviour
{
    [Header("World Settings")]
    [SerializeField] private int worldSeed = 0; // 🌱 User-defined or auto-generated seed
    [SerializeField] private bool autoChangeSeed = true; // 🔄 Toggle auto-changing seed

    [Header("Biome Settings")]
    [SerializeField] private List<Biome> biomes = new List<Biome>();

    private ChunkGenerator chunkGenerator;
    private ChunkClimate chunkClimate;

    private void Awake()
    {
        chunkGenerator = FindObjectOfType<ChunkGenerator>();
        chunkClimate = FindObjectOfType<ChunkClimate>();

        if (chunkGenerator == null)
        {
            Debugging.LogError("❌ `ChunkGenerator` not found!");
            return;
        }

        if (chunkClimate == null)
        {
            Debugging.LogWarning("⚠️ `ChunkClimate` not found! Creating one...");
            chunkClimate = gameObject.AddComponent<ChunkClimate>();
        }

        Debugging.LogOperation("✅ World Generator Initialized.");
    }

    private void Start()
    {
        RegenerateWorld(); // ✅ Ensure world is generated on start
    }

    public void RegenerateWorld()
    {
        Debugging.LogOperation("🔄 Regenerating World...");

        // ✅ Ensure old world data is cleared before generating a new one
        if (chunkGenerator != null)
        {
            chunkGenerator.ClearWorldData();
        }

        if (autoChangeSeed || worldSeed == 0) // ✅ Always change the seed if auto-change is enabled or seed is 0
        {
            worldSeed = Random.Range(int.MinValue, int.MaxValue); // 🎲 Generate new seed on every regenerate
        }

        Random.InitState(worldSeed); // 🌍 Apply seed globally to ensure deterministic results
        Debugging.LogOperation($"🌍 New World Seed: {worldSeed}");

        chunkGenerator.SetWorldSeed(worldSeed); // 🌱 Pass new seed to Chunk Generator
        chunkGenerator.RegenerateChunks();
    }

    public void GenerateClimateData()
    {
        if (chunkGenerator == null || chunkGenerator.GetChunks().Count == 0)
        {
            Debugging.LogError("❌ Cannot generate climate data: No chunks exist.");
            return;
        }

        Debugging.LogOperation("🌡️ Generating Climate Data for Chunks...");

        foreach (var chunk in chunkGenerator.GetChunks())
        {
            chunkClimate.GenerateClimateData(chunk, worldSeed); // 🌱 Pass the seed to climate generation
            AssignBiome(chunk);
        }

        Debugging.LogOperation("✅ Climate & Biome Assignment Complete!");
    }

    private void AssignBiome(ChunkData chunk)
    {
        foreach (Biome biome in biomes)
        {
            if (chunk.Temperature >= biome.temperatureRange.x && chunk.Temperature <= biome.temperatureRange.y &&
                chunk.Humidity >= biome.humidityRange.x && chunk.Humidity <= biome.humidityRange.y)
            {
                chunk.AssignedBiome = biome;
                Debugging.LogOperation($"🌍 Assigned Biome: {biome.biomeName} to {chunk.ChunkName}");
                return;
            }
        }

        Debugging.LogWarning($"⚠️ No matching biome found for {chunk.ChunkName}. Assigning default biome.");
        chunk.AssignedBiome = biomes.Count > 0 ? biomes[0] : null;
    }
}
