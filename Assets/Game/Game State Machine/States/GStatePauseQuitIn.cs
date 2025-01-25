using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GStatePauseQuitIn : GStateBase
{
    public GStatePauseQuitIn(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_context == null) return;
        
        ScreenTransition.Instance.Close();
    }
}