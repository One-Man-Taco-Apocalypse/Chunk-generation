using UnityEngine; // ✅ Required for Vector3

public class ChunkData
{
    public string ChunkName { get; private set; }
    public Vector3 ChunkPosition { get; private set; } // ✅ Now recognized
    public int ChunkHash { get; private set; }

    public float Temperature { get; set; }
    public float Humidity { get; set; }
    public float FoliageDensity { get; set; }

    public Biome AssignedBiome { get; set; }

    public ChunkData(int x, int z, float chunkSize)
    {
        ChunkName = $"Chunk_{x}_{z}";
        ChunkPosition = new Vector3(x * chunkSize, 0, z * chunkSize);
        ChunkHash = ChunkName.GetHashCode();
    }
}
