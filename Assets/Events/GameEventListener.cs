using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : GameEventListenerContainer<IGameEventOpts>
{
}

[Serializable]
public class GameEventListenerContainer<T> : GameEventListenerBase where T : IGameEventOpts
{
    [SerializeField]
    protected UnityEventGeneric<T> _unityEventGeneric;

    public void RaiseEvent(T opts)
    {
        _unityEvent?.Invoke();
        _unityEventGeneric?.Invoke(opts);
    }
}

public class GameEventListenerBase : MonoBehaviour
{
    [SerializeField]
    private bool _listenerRegistered = false;
    [SerializeField]
    protected GameEvent _gameEvent;
    [SerializeField]
    protected UnityEvent _unityEvent;
    private int _registerRetries = 10;
    private int _registerAttempts = 0;

    private void Awake()
    {
        StartCoroutine(RegisterAsync());
    }

    private void OnEnable()
    {
        StartCoroutine(RegisterAsync());
    }

    private void OnDisable()
    {
        _gameEvent?.Deregister(this);
        _listenerRegistered = false;
    }

    private IEnumerator RegisterAsync()
    {
        // halt if listener is already registered
        if (_listenerRegistered) yield break;

        // attempt to register the listener
        // this is spread across frames to account for out-of-order scene initialization
        while (!_listenerRegistered && _registerAttempts < _registerRetries)
        {
            _registerAttempts++;

            if (_gameEvent == null)
            {
                if (_gameEvent._debug && Application.isPlaying)
                {
                    Debug.LogWarning(
                        $"EventListener: _gameEvent is null: name({gameObject.name}), attempt number ({_registerAttempts}/{_registerRetries})",
                        gameObject);
                }

                // attempt to register again next frame
                yield return null;
                continue;
            }

            if (_gameEvent._debug && Application.isPlaying)
            {
                Debug.Log($"EventListener: Register listener: name({gameObject.name})", gameObject);
            }

            // register the listener with the event
            _gameEvent?.Register(this);
            _listenerRegistered = true;
        }

        if (!_listenerRegistered)
        {
            Debug.LogWarning(
                $"EventListener: unable to register _gameEvent after ({_registerAttempts}/{_registerRetries}) attempts: name({gameObject.name})",
                gameObject);
        }
    }

    public void RaiseEvent()
    {
        _unityEvent?.Invoke();
    }
}