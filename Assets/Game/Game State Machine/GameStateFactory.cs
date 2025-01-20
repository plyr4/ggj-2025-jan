public class GStateFactory : StateFactory
{
    public GStateFactory(StateMachineMono context) : base(context)
    {
    }

    public GStateBase Null()
    {
        return new GStateNull(_context, this);
    }

    public GStateBase Init()
    {
        return new GStateInit(_context, this);
    }

    public GStateBase StartIn()
    {
        return new GStateStartIn(_context, this);
    }

    public GStateBase Start()
    {
        return new GStateStart(_context, this);
    }

    public GStateBase StartOutPlayIn()
    {
        return new GStateStartOutPlayIn(_context, this);
    }
    
    public GStateBase StartOutQuitIn()
    {
        return new GStateStartOutQuitIn(_context, this);
    }

    public GStateBase IntroLoad()
    {
        return new GStateIntroLoad(_context, this);
    }
    
    public GStateBase Intro()
    {
        return new GStateIntro(_context, this);
    }

    public GStateBase PlayLoad()
    {
        return new GStatePlayLoad(_context, this);
    }

    public GStateBase PlayIn()
    {
        return new GStatePlayIn(_context, this);
    }
    
    public GStateBase Play()
    {
        return new GStatePlay(_context, this);
    }

    public GStateBase Pause()
    {
        return new GStatePause(_context, this);
    }

    public GStateBase PauseQuit()
    {
        return new GStatePauseQuit(_context, this);
    }

    public GStateBase GameOver()
    {
        return new GStateGameOver(_context, this);
    }

    public GStateBase RetryIn()
    {
        return new GStateRetryIn(_context, this);
    }

    public GStateBase Retry()
    {
        return new GStateRetry(_context, this);
    }
    public GStateBase Quit()
    {
        return new GStateQuit(_context, this);
    }
}