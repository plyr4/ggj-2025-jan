public class GStateMachine : StateMachine
{
    public GStateBase _currentGState;
    public GameEvent _onStateChange;

    public GStateMachine(GameEvent onStateChange)
    {
        _onStateChange = onStateChange;
    }

    public override void OnGameStateChange(IState previousState, IState newState)
    {
        _currentGState = (GStateBase)newState;
        if (_onStateChange != null)
            _onStateChange.Invoke(new GameStateChangeOpts(previousState, newState));
    }
}