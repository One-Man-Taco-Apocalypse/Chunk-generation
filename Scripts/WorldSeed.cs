using UnityEngine;
using System;

[ExecuteAlways]
public class WorldSeed : MonoBehaviour
{
    [Header("World Seed Settings")]
    [Tooltip("The current world seed value.")]
    [SerializeField] private int worldSeed = 0;

    /// <summary>
    /// Public read-only property to access the current seed.
    /// </summary>
    public int Seed => worldSeed;

    /// <summary>
    /// Regenerates the world seed using a new GUID's hash code.
    /// </summary>
    public void RegenerateSeed()
    {
        worldSeed = Mathf.Abs(Guid.NewGuid().GetHashCode());
        Debug.Log("New world seed generated: " + worldSeed);
    }
}
