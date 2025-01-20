public abstract class StateBase : IState
{
    public StateMachineMono _context;
    public StateFactory _factory;
    public bool _done;
    public bool _ready;

    protected StateBase()
    {
    }

    protected StateBase(StateMachineMono context, StateFactory factory)
    {
        _context = context;
        _factory = factory;
    }

    public virtual void OnEnter()
    {
        _done = false;
        _ready = false;
    }

    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void OnExit()
    {
        _done = false;
        _ready = false;
    }
}