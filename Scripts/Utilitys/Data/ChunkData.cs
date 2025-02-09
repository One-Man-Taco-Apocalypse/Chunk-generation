using UnityEngine;

/// <summary>
/// Data container for an individual chunk.
/// </summary>
public class ChunkData
{
    public string ChunkName { get; private set; }
    public Vector3 ChunkPosition { get; private set; }
    public int ChunkHash { get; private set; }

    public float Temperature { get; set; }
    public float Humidity { get; set; }
    public float FoliageDensity { get; set; }

    public Biome AssignedBiome { get; set; }

    public float ChunkSize { get; private set; }
    public int XIndex { get; private set; }
    public int ZIndex { get; private set; }

    public ChunkData(int x, int z, float chunkSize)
    {
        XIndex = x;
        ZIndex = z;
        ChunkName = $"Chunk_{x}_{z}";
        ChunkPosition = new Vector3(x * chunkSize, 0, z * chunkSize);
        ChunkHash = ChunkName.GetHashCode();
        ChunkSize = chunkSize;
    }
}
