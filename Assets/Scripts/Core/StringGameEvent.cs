using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StringEvent", menuName = "DungeonCrawler/Events/StringEvent")]
public class StringGameEvent : ScriptableObject
{
    private List<StringGameEventListener> _listeners = new List<StringGameEventListener>();

    public void RegisterListener(StringGameEventListener listener)
    {
        if (!_listeners.Contains(listener))
        {
            _listeners.Add(listener);
        }
    }

    public void UnregisterListener(StringGameEventListener listener)
    {
        if (_listeners.Contains(listener))
        {
            _listeners.Remove(listener);
        }
    }

    public void Raise(string data)
    {
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            _listeners[i].OnEventRaised(data);
        }
    }
}