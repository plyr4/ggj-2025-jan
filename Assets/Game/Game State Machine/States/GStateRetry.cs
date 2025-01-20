using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GStateRetry : GStateBase
{
    public GStateRetry(StateMachineMono context, StateFactory factory) : base(context, factory)
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
        
            // ScreenTransition.Instance.DelayedOpen(1f);   
        }
    }

    IEnumerator unloadScene()
    {
        SceneManager.UnloadSceneAsync("Play");
        yield return new WaitForSecondsRealtime(2f);
        SceneManager.LoadScene("Play", LoadSceneMode.Additive);
        // ScreenTransition.Instance.DelayedOpen(1f);  
    }
    
    public override void OnExit()
    {
        base.OnExit();

        if (_context == null) return;
    }
}