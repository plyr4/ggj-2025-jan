public interface ITransition
{
    public IState _to { get; }
    public IPredicate _condition { get; }
}