using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IGameEventOpts
{
}

[Serializable]
public class UnityEventGeneric<T> : UnityEvent<T> where T : IGameEventOpts
{
}

[CreateAssetMenu(menuName = "Game Event", fileName = "New Game Event")]
public class GameEvent : ScriptableObject
{
    protected ConcurrentDictionary<int, GameEventListenerBase> _listeners =
        new ConcurrentDictionary<int, GameEventListenerBase>();
    public bool _debug;

    public void Invoke()
    {
        if (_debug && Application.isPlaying)
        {
            Debug.Log($"v: Invoked GameEvent listeners: num_listeners({_listeners.Values.Count})");
        }

        foreach (var listener in _listeners)
        {
            if (_debug && Application.isPlaying)
            {
                Debug.Log($"GameEvent: RaiseEvent GameEvent listener: name({listener.Value.gameObject.name})",
                    listener.Value.gameObject);
            }

            listener.Value.RaiseEvent();
        }
    }

    public void Invoke(IGameEventOpts opts)
    {
        if (_debug && Application.isPlaying)
            Debug.Log($"GameEvent: Invoked GameEvent listeners: num_listeners({_listeners.Values.Count})");

        foreach (KeyValuePair<int, GameEventListenerBase> listener in _listeners)
        {
            if (_debug && Application.isPlaying)
                Debug.Log($"GameEvent: RaiseEvent GameEvent listener: name({listener.Value.gameObject.name})",
                    listener.Value.gameObject);
            (listener.Value as GameEventListener)?.RaiseEvent(opts);
        }
    }

    public void Deregister(GameEventListenerBase listener) =>
        _listeners.TryRemove(listener.gameObject.GetInstanceID(), out _);

    public void Register(GameEventListenerBase listener) =>
        _listeners.TryAdd(listener.gameObject.GetInstanceID(), listener);
}