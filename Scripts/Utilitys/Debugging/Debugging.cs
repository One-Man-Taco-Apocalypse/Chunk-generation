using UnityEngine;

/// <summary>
/// A singleton utility for debugging messages and settings.
/// </summary>
public class Debugging : MonoBehaviour
{
    private static Debugging instance;

    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugging = true;
    [SerializeField] private bool showChunkOutlines = true;
    [SerializeField] private Color chunkOutlineColor = Color.green;
    [SerializeField] private bool showChunkNames = true;
    [SerializeField] private Color chunkNameColor = Color.white;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static bool IsDebuggingEnabled => instance != null && instance.enableDebugging;
    public static bool ShowChunkOutlines => instance != null && instance.showChunkOutlines;
    public static Color GetChunkOutlineColor() => instance != null ? instance.chunkOutlineColor : Color.green;
    public static bool ShowChunkNames => instance != null && instance.showChunkNames;
    public static Color GetChunkNameColor() => instance != null ? instance.chunkNameColor : Color.white;

    public static void LogOperation(string message, string color = "#2ecc71")
    {
        if (!IsDebuggingEnabled)
            return;
        Debug.Log($"<b><color=#3498db>🟢 [Chunk Debug]</color></b> <color={color}>{message}</color>");
    }

    public static void LogWarning(string message)
    {
        if (!IsDebuggingEnabled)
            return;
        Debug.LogWarning($"<b><color=#f1c40f>🟡 [Chunk Debug]</color></b> {message}");
    }

    public static void LogError(string message)
    {
        if (!IsDebuggingEnabled)
            return;
        Debug.LogError($"<b><color=#e74c3c>🔴 [Chunk Debug]</color></b> {message}");
    }
}
