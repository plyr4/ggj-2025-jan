using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GUIHowToPlayMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _viewParent;
    [SerializeField]
    private Vector2 _offScreenPosition = new Vector2(0, -800f);
    [SerializeField]
    private float _animationDuration = 0.5f;
    [SerializeField]
    private Ease _animationEaseIn = Ease.InOutSine;
    [SerializeField]
    private Ease _animationEaseOut = Ease.InOutSine;
    public GameEvent _menuInDone;
    public GameEvent _menuOutDone;
    public GameEvent _menuOut;
    public Tween _menuTween;

    public void Update()
    {
        if (GameInput.Instance._menuBackPressed)
        {
            switch (GStateMachineGame.Instance.CurrentState())
            {
                case GStateHowToPlay _:
                    _menuOut.Invoke();
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        if (_menuTween != null) _menuTween.Kill();
    }

    public void HandleGameStateChange(IGameEventOpts opts)
    {
        GameStateChangeOpts opts_ = (GameStateChangeOpts)opts;

        switch (opts_._newState)
        {
            case GStateInit _:
                _viewParent.SetActive(false);
                _viewParent.transform.localPosition = _offScreenPosition;
                break;
            case GStateHowToPlayIn _:
                _viewParent.SetActive(true);
                _viewParent.GetComponentInChildren<UnityEngine.UI.Button>().Select();
                if (_menuTween != null) _menuTween.Kill();
                _menuTween = _viewParent.transform.DOLocalMove(Vector3.zero, _animationDuration)
                    .SetUpdate(true)
                    .OnComplete(() => { _menuInDone.Invoke(); })
                    .SetEase(_animationEaseIn);
                break;
            case GStateHowToPlayOut _:
                if (_menuTween != null) _menuTween.Kill();
                _menuTween = _viewParent.transform.DOLocalMove(_offScreenPosition, _animationDuration)
                    .SetUpdate(true)
                    .OnComplete(() => { _menuOutDone.Invoke(); })
                    .SetEase(_animationEaseOut);
                break;

            case GStateStart _:
            case GStatePause _:
                _viewParent.SetActive(false);
                break;
        }
    }
}