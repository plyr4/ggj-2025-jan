using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GStatePauseQuit : GStateBase
{
    public GStatePauseQuit(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_context == null) return;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}