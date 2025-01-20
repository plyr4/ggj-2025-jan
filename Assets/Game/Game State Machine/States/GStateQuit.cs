using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GStateQuit : GStateBase
{
    public GStateQuit(StateMachineMono context, StateFactory factory) : base(context, factory)
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