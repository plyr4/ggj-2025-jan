public class GameStateChangeOpts : IGameEventOpts
{
    public IState _newState;
    public IState _previousState;

    public GameStateChangeOpts(IState previousState, IState newState)
    {
        _previousState = previousState;
        _newState = newState;
    }

    public IState PreviousState()
    {
        return _previousState;
    }

    public IState NewState()
    {
        return _newState;
    }
}