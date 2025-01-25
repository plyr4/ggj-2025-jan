using UnityEngine;


public class GUIStartMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _viewParent;

    public void HandleGameStateChange(IGameEventOpts opts)
    {
        GameStateChangeOpts opts_ = (GameStateChangeOpts)opts;

        switch (opts_._newState)
        {
            case GStateStartIn _:
                _viewParent.SetActive(true);
                _viewParent.GetComponentInChildren<UnityEngine.UI.Button>().Select();
                break;
            case GStatePlayLoad _:
                _viewParent.SetActive(false);
                break;
        }
    }
}