public class GStateIntro : GStateBase
{
    public GStateIntro(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_context == null) return;

#if UNITY_EDITOR
        // if (_context._skipIntroInEditMode) _context._introDone = true;
#endif

        // ScreenTransition.Instance.DelayedOpen(0.5f);
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_context == null) return;
    }
}