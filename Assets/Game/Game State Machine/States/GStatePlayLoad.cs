using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GStatePlayLoad : GStateBase
{
    public GStatePlayLoad(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_context == null) return;

        Scene play = SceneManager.GetSceneByName("Play");
        if (play != null && play.isLoaded)
        {
            // CoroutineRunner.instance.StartCoroutine(unloadScene());
        }
        else
        {
            SceneManager.LoadScene("Play", LoadSceneMode.Additive);

            // todo: actually handle waiting for this to be done loading
            // for now, its just a wait time of 1 second
            // ScreenTransition.Instance.Open(1f);   
        }

        _context.StartCoroutine(delayedDone());
    }

    IEnumerator unloadScene()
    {
        SceneManager.UnloadSceneAsync("Play");
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene("Play", LoadSceneMode.Additive);
        // ScreenTransition.Instance.DelayedOpen(1f);  
    }

    IEnumerator delayedDone()
    {
        yield return new WaitForSecondsRealtime(1f);
        _done = true;
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_context == null) return;
    }
}