using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewBiome", menuName = "World Generation/Biome")]
public class Biome : ScriptableObject
{
    [Header("General Settings")]
    public string biomeName = "New Biome";
    public Color biomeColor = Color.green;

    [Header("Foliage Settings")]
    [Range(0, 100)] public float foliageDensity = 50f;
    [MinMaxSlider(0, 100)] public Vector2 foliageRange = new Vector2(30, 70);

    [Header("Temperature Settings")]
    [MinMaxSlider(-50, 50)] public Vector2 temperatureRange = new Vector2(0, 30);

    [Header("Humidity Settings")]
    [MinMaxSlider(0, 100)] public Vector2 humidityRange = new Vector2(20, 80);

    [Header("Probability Settings")]
    [Tooltip("Higher values make this biome more likely to spawn.")]
    public float spawnWeight = 1.0f;
}
