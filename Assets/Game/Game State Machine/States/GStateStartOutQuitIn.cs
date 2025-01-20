public class GStateStartOutQuitIn : GStateBase
{
    public GStateStartOutQuitIn(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_context == null) return;
        
        ScreenTransition.Instance.Close();
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_context == null) return;
    }
}