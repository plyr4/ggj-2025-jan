using System.Collections.Generic;
using UnityEngine;

public class GStateMachineGame : GStateMachineMono
{
    private static GStateMachineGame _instance;

    private GStateBase _nan;
    private GStateBase _init;
    private GStateBase _startIn;
    private GStateBase _start;
    private GStateBase _quit;
    private GStateBase _startOutPlayIn;
    private GStateBase _startOutQuitIn;
    private GStateBase _playLoad;
    private GStateBase _playIn;
    private GStateBase _play;
    private GStateBase _pause;
    private GStateBase _pauseRetryIn;
    private GStateBase _pauseRetry;
    private GStateBase _pauseQuitIn;
    private GStateBase _pauseQuit;
    private GStateBase _settingsIn;
    private GStateBase _settings;
    private GStateBase _settingsOut;
    private GStateBase _howToPlay;
    private GStateBase _howToPlayIn;
    private GStateBase _howToPlayOut;


    public bool _unloadPlaySceneOnApplicationStart = true;

    public static GStateMachineGame Instance
    {
        get
        {
            // attempt to locate the singleton
            if (_instance == null) _instance = (GStateMachineGame)FindObjectOfType(typeof(GStateMachineGame));

            if (_instance == null)
            {
                Debug.LogError("no GStateMachineGame instance found in scene, this will cause issues");
                _instance = new GameObject("GStateMachineGame").AddComponent<GStateMachineGame>();
            }

            // return singleton
            return _instance;
        }
    }

    public override void Start()
    {
        base.Start();

        Initialize(new GStateMachine(_onStateChange), new GStateFactory(this));
    }

    protected override void Initialize(StateMachine stateMachine, StateFactory factory)
    {
        // set up the state machine and state factory
        base.Initialize(stateMachine, factory);

        // states
        _nan = ((GStateFactory)_stateFactory).Null();
        _init = ((GStateFactory)_stateFactory).Init();
        _startIn = ((GStateFactory)_stateFactory).StartIn();
        _start = ((GStateFactory)_stateFactory).Start();
        _startOutPlayIn = ((GStateFactory)_stateFactory).StartOutPlayIn();
        _startOutQuitIn = ((GStateFactory)_stateFactory).StartOutQuitIn();
        _playLoad = ((GStateFactory)_stateFactory).PlayLoad();
        _playIn = ((GStateFactory)_stateFactory).PlayIn();
        _play = ((GStateFactory)_stateFactory).Play();
        _pause = ((GStateFactory)_stateFactory).Pause();
        _pauseRetryIn = ((GStateFactory)_stateFactory).PauseRetryIn();
        _pauseRetry = ((GStateFactory)_stateFactory).PauseRetry();
        _quit = ((GStateFactory)_stateFactory).Quit();
        _pauseQuitIn = ((GStateFactory)_stateFactory).PauseQuitIn();
        _pauseQuit = ((GStateFactory)_stateFactory).PauseQuit();
        _settingsIn = ((GStateFactory)_stateFactory).SettingsIn();
        _settings = ((GStateFactory)_stateFactory).Settings();
        _settingsOut = ((GStateFactory)_stateFactory).SettingsOut();
        _howToPlayIn = ((GStateFactory)_stateFactory).HowToPlayIn();
        _howToPlay = ((GStateFactory)_stateFactory).HowToPlay();
        _howToPlayOut = ((GStateFactory)_stateFactory).HowToPlayOut();

        // transitions
        at(_nan, _init, new FuncPredicate(() =>
            true
        ));

        at(_init, _startIn, new FuncPredicate(() =>
            _init._done
        ));

        at(_startIn, _start, new FuncPredicate(() =>
            _startIn._done
        ));

        at(_start, _startOutPlayIn, new FuncPredicate(() =>
                _startOutPlayIn._ready
#if UNITY_EDITOR
                || _skipStartInEditMode
#endif
        ));

        at(_startOutPlayIn, _playLoad, new FuncPredicate(() =>
            _startOutPlayIn._done
        ));

        at(_start, _startOutQuitIn, new FuncPredicate(() =>
            _startOutQuitIn._ready
        ));

        at(_startOutQuitIn, _quit, new FuncPredicate(() =>
            _startOutQuitIn._done
        ));

        at(_playLoad, _playIn, new FuncPredicate(() =>
            _playLoad._done
        ));

        at(_playIn, _play, new FuncPredicate(() =>
            _playIn._done
        ));

        at(_play, _pause, new FuncPredicate(() =>
            _pause._ready
        ));

        at(_pause, _play, new FuncPredicate(() =>
            _pause._done && _play._ready
        ));

        at(_pause, _pauseRetryIn, new FuncPredicate(() =>
            _pause._done && _pauseRetryIn._ready
        ));

        at(_pauseRetryIn, _pauseRetry, new FuncPredicate(() =>
            _pauseRetryIn._done
        ));

        at(_pauseRetry, _playIn, new FuncPredicate(() =>
            _pauseRetry._done
        ));

        at(_pause, _pauseQuitIn, new FuncPredicate(() =>
            _pause._done && _pauseQuitIn._ready
        ));

        at(_pauseQuitIn, _quit, new FuncPredicate(() =>
            _pauseQuitIn._done
        ));

        at(_start, _settingsIn, new FuncPredicate(() =>
            _start._done && _settingsIn._ready
        ));

        at(_pause, _settingsIn, new FuncPredicate(() =>
            _pause._done && _settingsIn._ready
        ));

        at(_settingsIn, _settings, new FuncPredicate(() =>
            _settingsIn._done
        ));

        at(_settings, _settingsOut, new FuncPredicate(() =>
            _settingsOut._ready
        ));

        at(_settingsOut, _start, new FuncPredicate(() =>
            _settingsOut._done && _start._ready
        ));

        at(_settingsOut, _pause, new FuncPredicate(() =>
            _settingsOut._done && _pause._ready
        ));

        at(_start, _howToPlayIn, new FuncPredicate(() =>
            _start._done && _howToPlayIn._ready
        ));

        at(_pause, _howToPlayIn, new FuncPredicate(() =>
            _pause._done && _howToPlayIn._ready
        ));

        at(_howToPlayIn, _howToPlay, new FuncPredicate(() =>
            _howToPlayIn._done
        ));

        at(_howToPlay, _howToPlayOut, new FuncPredicate(() =>
            _howToPlayOut._ready
        ));

        at(_howToPlayOut, _start, new FuncPredicate(() =>
            _howToPlayOut._done && _start._ready
        ));

        at(_howToPlayOut, _pause, new FuncPredicate(() =>
            _howToPlayOut._done && _pause._ready
        ));

        _stateMachine.SetState(_nan);
    }

    public GStateBase CurrentState()
    {
        if (!Application.isPlaying) return new GStateNull();
        if (_stateMachine == null) return new GStateNull();
        if (_stateMachine._currentGState == null) return new GStateNull();
        return _stateMachine._currentGState;
    }

    public bool ContainsCurrentState(List<GStateBase> states)
    {
        GStateBase state = CurrentState();
        if (state == null) return false;
        if (states == null) return false;
        foreach (GStateBase s in states)
            if (state.GetType() == s.GetType())
                return true;
        return false;
    }

    public void HandleTransitionCloseDoneEvent()
    {
        switch (_stateMachine._current._state)
        {
            case GStateStartOutPlayIn:
                _startOutPlayIn._done = true;
                break;
            case GStateStartOutQuitIn:
                _startOutQuitIn._done = true;
                break;
            case GStatePauseRetryIn:
                _pauseRetryIn._done = true;
                break;
            case GStatePauseQuitIn:
                _pauseQuitIn._done = true;
                break;
        }
    }

    public void HandleTransitionOpenDoneEvent()
    {
        switch (_stateMachine._current._state)
        {
            case GStatePlayIn:
                _playIn._done = true;
                break;
            case GStatePlayLoad:
                break;
            case GStatePauseRetry:
                break;
        }
    }

    public void HandleStartPlay()
    {
        _startOutPlayIn._ready = true;
    }

    public void HandlePlayPause()
    {
        _pause._ready = true;
    }

    public void HandlePausePlay()
    {
        _pause._done = true;
        _play._ready = true;
    }

    public void HandlePauseRetry()
    {
        _pause._done = true;
        _pauseRetryIn._ready = true;
    }

    public void HandlePauseQuit()
    {
        _pause._done = true;
        _pauseQuitIn._ready = true;
    }

    public void HandleStartQuit()
    {
        _startOutQuitIn._ready = true;
    }

    public void HandleSettingsIn()
    {
        switch (CurrentState())
        {
            case GStateStart _:
                _start._done = true;
                break;
            case GStatePause _:
                _pause._done = true;
                break;
        }

        _settingsIn._ready = true;
    }

    public void HandleSettingsInDone()
    {
        _settingsIn._done = true;
    }

    public void HandleSettingsOut()
    {
        _settingsOut._ready = true;
    }

    public void HandleSettingsOutDone()
    {
        switch (_settingsInState)
        {
            case GStateStart _:
                _start._ready = true;
                break;
            case GStatePause _:
                _pause._ready = true;
                break;
        }

        _settingsInState = null;

        _settingsOut._done = true;
    }


    public void HandleHowToPlayIn()
    {
        switch (CurrentState())
        {
            case GStateStart _:
                _start._done = true;
                break;
            case GStatePause _:
                _pause._done = true;
                break;
        }

        _howToPlayIn._ready = true;
    }

    public void HandleHowToPlayInDone()
    {
        _howToPlayIn._done = true;
    }

    public void HandleHowToPlayOut()
    {
        _howToPlayOut._ready = true;
    }

    public void HandleHowToPlayOutDone()
    {
        switch (_howToPlayInState)
        {
            case GStateStart _:
                _start._ready = true;
                break;
            case GStatePause _:
                _pause._ready = true;
                break;
        }

        _howToPlayInState = null;

        _howToPlayOut._done = true;
    }
}