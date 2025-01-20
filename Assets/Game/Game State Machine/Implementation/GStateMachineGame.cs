using System.Collections.Generic;
using UnityEngine;

public class GStateMachineGame : GStateMachineMono
{
    private static GStateMachineGame _instance;

    private GStateBase _nan;
    private GStateBase _init;
    private GStateBase _startIn;
    private GStateBase _start;
    private GStateBase _startOutPlayIn;
    private GStateBase _startOutQuitIn;
    private GStateBase _playLoad;
    private GStateBase _playIn;
    private GStateBase _play;
    private GStateBase _pause;
    private GStateBase _retry;
    private GStateBase _quit;

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
        _retry = ((GStateFactory)_stateFactory).Retry();
        _quit = ((GStateFactory)_stateFactory).Quit();

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

        at(_pause, _retry, new FuncPredicate(() =>
            _pause._done && _retry._ready
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
            case GStateRetry:
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
        _retry._ready = true;
    }

    public void HandleStartQuit()
    {
        _startOutQuitIn._ready = true;
    }
}