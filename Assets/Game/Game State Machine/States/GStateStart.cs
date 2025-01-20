public class GStateStart : GStateBase
{
    public GStateStart(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_context == null) return;

#if UNITY_EDITOR
        // if (_context._skipStartInEditMode) _context._startOnPlay = true;
#endif
    }
}