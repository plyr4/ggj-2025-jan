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

    public GStateBase PauseQuitIn()
    {
        return new GStatePauseQuitIn(_context, this);
    }

    public GStateBase PauseQuit()
    {
        return new GStatePauseQuit(_context, this);
    }

    public GStateBase GameOver()
    {
        return new GStateGameOver(_context, this);
    }

    public GStateBase PauseRetryIn()
    {
        return new GStatePauseRetryIn(_context, this);
    }

    public GStateBase PauseRetry()
    {
        return new GStatePauseRetry(_context, this);
    }

    public GStateBase SettingsIn()
    {
        return new GStateSettingsIn(_context, this);
    }

    public GStateBase SettingsOut()
    {
        return new GStateSettingsOut(_context, this);
    }

    public GStateBase Settings()
    {
        return new GStateSettings(_context, this);
    }

    public GStateBase HowToPlayIn()
    {
        return new GStateHowToPlayIn(_context, this);
    }

    public GStateBase HowToPlayOut()
    {
        return new GStateHowToPlayOut(_context, this);
    }

    public GStateBase HowToPlay()
    {
        return new GStateHowToPlay(_context, this);
    }

    public GStateBase Quit()
    {
        return new GStateQuit(_context, this);
    }
}