using System.Collections;
using UnityEngine.SceneManagement;

public class GStateInit : GStateBase
{
    public GStateInit(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_context == null) return;

        InitializeGame();

        ScreenTransition.Instance.CloseImmediate();
        ScreenTransition.Instance.Open(0.25f);
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_context == null) return;
    }

    private void InitializeGame()
    {
        UnloadNonCoreScenes();
        // delay to next frame to account for immediate loading (ex: core scenes were pre-loaded)
        _context.StartCoroutine(finishLoadingCoreScenesNextFrame());
    }

    private void UnloadNonCoreScenes()
    {
        // unload non-core scenes
        // but only if scene is loaded
        if (SceneManager.GetSceneByName("Play").isLoaded)
        {
            SceneManager.UnloadSceneAsync("Play");
        }
    }

    private IEnumerator finishLoadingCoreScenesNextFrame()
    {
        yield return null;
        FinishLoadingCoreScenes();
    }

    private void FinishLoadingCoreScenes()
    {
        // entrypoint to the actual interactive application
        _done = true;
    }
}