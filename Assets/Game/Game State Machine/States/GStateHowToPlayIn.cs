using UnityEngine;

public class GStateHowToPlayIn : GStateBase
{
    public GStateHowToPlayIn(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public override void OnEnter()
    {
        _context._howToPlayInState = GStateMachineGame.Instance.CurrentState();

        base.OnEnter();

        if (_context == null) return;

        Time.timeScale = 0f;
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_context == null) return;
    }
}