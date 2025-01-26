using UnityEngine;

public class GStateSettings : GStateBase
{
    public GStateSettings(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_context == null) return;
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_context == null) return;
    }
}