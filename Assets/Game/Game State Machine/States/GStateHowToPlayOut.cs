using UnityEngine;

public class GStateHowToPlayOut : GStateBase
{
    public GStateHowToPlayOut(StateMachineMono context, StateFactory factory) : base(context, factory)
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

        Time.timeScale = 1f;
    }
}