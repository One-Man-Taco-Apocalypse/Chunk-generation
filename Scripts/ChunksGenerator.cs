using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ChunkGenerator : MonoBehaviour
{
    [Header("World Settings")]
    [SerializeField] private int worldSizeInChunks = 5;
    [SerializeField] private float chunkSize = 10f;
    
    // New field to control how many chunks are generated per frame.
    [SerializeField] private int chunksPerFrame = 10;

    // Expose the total number of chunks along one axis.
    public int WorldSizeInChunks => worldSizeInChunks;

    // Public property to allow others to check if generation is still in progress.
    public bool IsGenerating => isGenerating;

    private Transform chunksParent;
    private bool isGenerating = false;
    private readonly HashSet<ChunkData> chunkList = new HashSet<ChunkData>();

    private WorldGenerator worldGenerator;

    private void Awake()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
    }

    /// <summary>
    /// Regenerates all chunks.
    /// Uses a coroutine in Play mode (with a yield every [chunksPerFrame] chunks) 
    /// and synchronous generation in Editor mode.
    /// </summary>
    public void RegenerateChunks()
    {
        if (isGenerating)
        {
            Debug.LogWarning("⚠️ Chunk generation already in progress.");
            return;
        }
        ClearWorldData();

        if (Application.isPlaying)
        {
            StartCoroutine(GenerateChunksCoroutine());
        }
        else
        {
            GenerateChunksImmediate();
        }
    }

    /// <summary>
    /// Generates chunks using a coroutine (Play mode).
    /// Generates a specified number of chunks per frame to spread out the workload.
    /// </summary>
    private IEnumerator GenerateChunksCoroutine()
    {
        isGenerating = true;
        Debug.Log("🟢 Generating chunks...");

        chunkList.Clear();
        if (chunksParent != null)
        {
            DestroyImmediate(chunksParent.gameObject);
            chunksParent = null;
        }
        chunksParent = new GameObject("Chunks").transform;

        int halfSize = worldSizeInChunks / 2;
        int counter = 0;
        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int z = -halfSize; z <= halfSize; z++)
            {
                // Create a new chunk.
                ChunkData newChunk = new ChunkData(x, z, chunkSize);
                chunkList.Add(newChunk);

                // Create a GameObject for visualization.
                GameObject chunkObject = new GameObject(newChunk.ChunkName);
                chunkObject.transform.position = newChunk.ChunkPosition;
                chunkObject.transform.SetParent(chunksParent);

                counter++;
                // Yield after generating chunksPerFrame chunks.
                if (counter >= chunksPerFrame)
                {
                    counter = 0;
                    yield return null;
                }
            }
        }
        isGenerating = false;
    }

    /// <summary>
    /// Synchronously generates chunks (Editor mode).
    /// </summary>
    private void GenerateChunksImmediate()
    {
        isGenerating = true;
        Debug.Log("🟢 Generating chunks immediately (Editor mode)...");

        chunkList.Clear();
        if (chunksParent != null)
        {
            DestroyImmediate(chunksParent.gameObject);
            chunksParent = null;
        }
        chunksParent = new GameObject("Chunks").transform;

        int halfSize = worldSizeInChunks / 2;
        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int z = -halfSize; z <= halfSize; z++)
            {
                ChunkData newChunk = new ChunkData(x, z, chunkSize);
                chunkList.Add(newChunk);

                GameObject chunkObject = new GameObject(newChunk.ChunkName);
                chunkObject.transform.position = newChunk.ChunkPosition;
                chunkObject.transform.SetParent(chunksParent);
            }
        }
        isGenerating = false;
    }

    public float GetChunkSize() => chunkSize;

    /// <summary>
    /// Clears all generated chunk GameObjects and data.
    /// </summary>
    public void ClearWorldData()
    {
        Debug.Log("🗑️ Clearing old world data...");
        if (chunksParent != null)
        {
            DestroyImmediate(chunksParent.gameObject);
            chunksParent = null;
        }
        chunkList.Clear();
    }

    /// <summary>
    /// Returns the set of all chunks.
    /// </summary>
    public HashSet<ChunkData> GetChunks() => chunkList;
}
