using UnityEngine;

public class GUIPlayHelp : MonoBehaviour
{
    [SerializeField]
    private GameObject _toggleParent;
    [SerializeField]
    private GameObject _viewParent;
    [SerializeField]
    private GameEvent _playPause;

    public void HandleGameStateChange(IGameEventOpts opts)
    {
        GameStateChangeOpts opts_ = (GameStateChangeOpts)opts;

        switch (opts_._newState)
        {
            case GStateInit _:
                _viewParent.SetActive(false);
                break;
            case GStatePlayIn _:
                _viewParent.SetActive(true);
                break;
        }
    }

    public void HandleToggle(bool value)
    {
        _toggleParent.SetActive(value);
    }

    public void HandleOnClickEscape()
    {
        _playPause.Invoke();
    }
}