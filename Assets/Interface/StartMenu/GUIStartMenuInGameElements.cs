using UnityEngine;

public class GUIStartMenuInGameElements : MonoBehaviour
{
    public GameObject _viewParent;
    public void HandleGameStateChange(IGameEventOpts opts)
    {
        GameStateChangeOpts opts_ = (GameStateChangeOpts)opts;

        switch (opts_._newState)
        {
            case GStateStartIn _:
                _viewParent.SetActive(true);
                break;
            case GStatePlayLoad _:
            case GStatePlayIn _:
                _viewParent.SetActive(false);
                break;
        }
    }
}
