using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [Header("World Settings")]
    [SerializeField] private int worldSizeInChunks = 5;
    [SerializeField] private float chunkSize = 10f;

    private Transform chunksParent;
    private bool isGenerating = false;
    private HashSet<ChunkData> chunkList = new HashSet<ChunkData>();
    private int worldSeed = 0; // 🌱 Stores the seed

    private WorldGenerator worldGenerator;

    private void Awake()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
    }

    public void SetWorldSeed(int seed)
    {
        worldSeed = seed;
        Random.InitState(worldSeed); // 🌍 Apply the seed for deterministic chunk generation
    }

    private void Start()
    {
        Debugging.LogOperation("🚀 Starting chunk generation...");
        StartCoroutine(GenerateChunks());
    }

    public void RegenerateChunks()
    {
        if (isGenerating)
        {
            Debugging.LogWarning("⚠️ Chunk regeneration skipped: Generation already in progress.");
            return;
        }

        Debugging.LogOperation("🔄 Regenerating Chunks...");
        ClearWorldData();
        StartCoroutine(GenerateChunks());
    }

    public IEnumerator GenerateChunks()
    {
        if (isGenerating)
        {
            Debugging.LogWarning("⚠️ Chunk generation already in progress. Skipping...");
            yield break;
        }

        isGenerating = true;
        Debugging.LogOperation($"🟢 Generating chunks with seed: {worldSeed}...");

        chunkList.Clear();
        if (chunksParent != null)
        {
            DestroyImmediate(chunksParent.gameObject);
            chunksParent = null;
        }

        chunksParent = new GameObject("Chunks").transform;

        Random.InitState(worldSeed); // 🌱 Apply seed before chunk placement
        int halfSize = worldSizeInChunks / 2;
        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int z = -halfSize; z <= halfSize; z++)
            {
                ChunkData newChunk = new ChunkData(x, z, chunkSize);
                chunkList.Add(newChunk);

                UnityThreadDispatcher.RunOnMainThread(() =>
                {
                    GameObject chunkObject = new GameObject(newChunk.ChunkName);
                    chunkObject.transform.position = newChunk.ChunkPosition;
                    chunkObject.transform.SetParent(chunksParent);
                });

                yield return null;
            }
        }

        isGenerating = false;

        if (worldGenerator != null)
        {
            worldGenerator.GenerateClimateData();
        }
    }

    public float GetChunkSize()
{
    return chunkSize;
}


    public void ClearWorldData()
    {
        Debugging.LogOperation("🗑️ Clearing old world data...");

        if (chunksParent != null)
        {
            DestroyImmediate(chunksParent.gameObject);
            chunksParent = null;
        }

        chunkList.Clear();
    }

    public HashSet<ChunkData> GetChunks()
    {
        return chunkList ?? new HashSet<ChunkData>();
    }
}
