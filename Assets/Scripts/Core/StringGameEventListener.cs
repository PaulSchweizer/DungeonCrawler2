using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class StringUnityEvent : UnityEvent<String> { }

public class StringGameEventListener : MonoBehaviour
{
    public StringGameEvent Event;
    public StringUnityEvent Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(string data)
    {
        Response.Invoke(data);
    }
}