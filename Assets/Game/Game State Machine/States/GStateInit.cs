using System.Collections;

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
        // delay to next frame to account for immediate loading (ex: core scenes were pre-loaded)
        _context.StartCoroutine(finishLoadingCoreScenesNextFrame());
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