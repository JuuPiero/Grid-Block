using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    private static readonly Dictionary<string, Action> events = new();

    public static void On(string eventName, Action callback)
    {
        if (events.ContainsKey(eventName))
            events[eventName] += callback;
        else
            events[eventName] = callback;
    }

    public static void Off(string eventName, Action callback)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName] -= callback;

            if (events[eventName] == null)
                events.Remove(eventName);
        }
    }

    public static void Emit(string eventName)
    {
        if (events.TryGetValue(eventName, out var callback))
        {
            callback?.Invoke();
            #if UNITY_EDITOR
            Debug.Log("Raise Event: " + eventName);
            #endif
        }
            
    }
}