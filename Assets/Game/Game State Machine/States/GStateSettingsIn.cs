using UnityEngine;

public class GStateSettingsIn : GStateBase
{
    public GStateSettingsIn(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public override void OnEnter()
    {
        _context._settingsInState = GStateMachineGame.Instance.CurrentState();

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