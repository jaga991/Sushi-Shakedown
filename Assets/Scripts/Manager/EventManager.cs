using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private Dictionary<string, Action<object>> eventDictionary = new Dictionary<string, Action<object>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    // Subscribe to an event
    public void Subscribe(string eventName, Action<object> listener)
    {
        if (!eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] = delegate { };
        }
        eventDictionary[eventName] += listener;
    }

    // Unsubscribe from an event
    public void Unsubscribe(string eventName, Action<object> listener)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] -= listener;
        }
    }

    // Trigger an event
    public void TriggerEvent(string eventName, object eventData = null)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName]?.Invoke(eventData);
        }
    }
}
