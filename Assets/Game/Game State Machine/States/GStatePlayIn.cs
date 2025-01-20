using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GStatePlayIn : GStateBase
{
    public GStatePlayIn(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_context == null) return;
        
        ScreenTransition.Instance.Open(0.5f);
    }
    
    public override void OnExit()
    {
        base.OnExit();

        if (_context == null) return;
    }
}