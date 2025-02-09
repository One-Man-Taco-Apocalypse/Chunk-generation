using System;
using System.Collections.Concurrent;
using UnityEngine;

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
            Destroy(gameObject); // Ensures only one instance exists
        }
    }

    private void Update()
    {
        while (actions.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }

    public static void RunOnMainThread(Action action)
    {
        if (action != null)
        {
            actions.Enqueue(action);
        }
    }
}
