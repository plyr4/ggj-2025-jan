using UnityEngine;

public abstract class StateMachineMono : MonoBehaviour
{
    protected StateMachine _stateMachine;
    protected StateFactory _stateFactory;
    [SerializeField]
    protected string _currentStateName;
    [SerializeField]
    private bool _showDebug;
    [SerializeField]
    protected string[] _states;
    [SerializeField]
    protected string[] _transitions;

    public virtual void Start()
    {
    }

    public virtual void Update()
    {
        if (_stateMachine == null) return;
        _stateMachine.Update();
        _currentStateName = _stateMachine._current?._state?.GetType().Name;
        if (_showDebug)
        {
            _states = _stateMachine.GetStates();
            _transitions = _stateMachine.GetTransitions();
        }
    }

    public virtual void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }

    protected virtual void Initialize(StateMachine stateMachine, StateFactory factory)
    {
        _stateMachine = stateMachine;
        _stateFactory = factory;
    }

    // a state transition with a from, to, and condition
    protected void at(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);

    // a state transition with a to and condition, but no from
    protected void any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
}