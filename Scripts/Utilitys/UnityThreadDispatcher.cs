using System;
using System.Collections.Concurrent;
using UnityEngine;

/// <summary>
/// Utility to dispatch actions on the Unity main thread.
/// </summary>
public class UnityThreadDispatcher : MonoBehaviour
{
    private static UnityThreadDispatcher instance;
    private static readonly ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        while (actions.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }

    /// <summary>
    /// Enqueues an action to be executed on the main thread.
    /// </summary>
    public static void RunOnMainThread(Action action)
    {
        if (action != null)
            actions.Enqueue(action);
    }
}
