public class GStatePlay : GStateBase
{
    public GStatePlay(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_context == null) return;
    }
}