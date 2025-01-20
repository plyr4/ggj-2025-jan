public class Transition : ITransition
{
    public IState _to { get; }
    public IPredicate _condition { get; }

    public Transition(IState to, IPredicate condition)
    {
        _to = to;
        _condition = condition;
    }
}