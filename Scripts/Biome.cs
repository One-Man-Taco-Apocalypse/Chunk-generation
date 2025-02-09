using UnityEngine;

[CreateAssetMenu(fileName = "NewBiome", menuName = "World Generation/Biome")]
public class Biome : ScriptableObject
{
    [Header("General Settings")]
    public string biomeName = "New Biome";
    public Color biomeColor = Color.green;

    [Header("Foliage Settings")]
    [Range(0, 100)] public float foliageDensity = 50f;

    [Header("Temperature Settings")]
    [MinMaxSlider(-50, 50)] public Vector2 temperatureRange = new Vector2(0, 30);

    [Header("Humidity Settings")]
    [MinMaxSlider(0, 100)] public Vector2 humidityRange = new Vector2(20, 80);
}
