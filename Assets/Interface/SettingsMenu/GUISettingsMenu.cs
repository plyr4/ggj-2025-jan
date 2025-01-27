using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GUISettingsMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _viewParent;
    [SerializeField]
    private Slider _volumeSlider;
    [SerializeField]
    private Vector2 _offScreenPosition = new Vector2(0, -800f);
    [SerializeField]
    private float _animationDuration = 0.5f;
    [SerializeField]
    private Ease _animationEaseIn = Ease.InOutSine;
    [SerializeField]
    private Ease _animationEaseOut = Ease.InOutSine;
    public GameEvent _settingsInDone;
    public GameEvent _settingsOutDone;
    public GameEvent _settingsOut;
    public Tween _menuTween;
    public GUIPlayHelp _guiPlayHelp;

    public void Update()
    {
        if (GameInput.Instance._menuBackPressed)
        {
            switch (GStateMachineGame.Instance.CurrentState())
            {
                case GStateSettings _:
                    _settingsOut.Invoke();
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
            case GStateSettingsIn _:
                _viewParent.SetActive(true);
                _viewParent.GetComponentInChildren<UnityEngine.UI.Button>().Select();
                if (_menuTween != null) _menuTween.Kill();
                _menuTween = _viewParent.transform.DOLocalMove(Vector3.zero, _animationDuration)
                    .SetUpdate(true)
                    .OnComplete(() => { _settingsInDone.Invoke(); })
                    .SetEase(_animationEaseIn);

                _volumeSlider.value = SoundManager.Instance._volume;
                break;
            case GStateSettingsOut _:
                if (_menuTween != null) _menuTween.Kill();
                _menuTween = _viewParent.transform.DOLocalMove(_offScreenPosition, _animationDuration)
                    .SetUpdate(true)
                    .OnComplete(() => { _settingsOutDone.Invoke(); })
                    .SetEase(_animationEaseOut);
                break;

            case GStateStart _:
            case GStatePause _:
                _viewParent.SetActive(false);
                break;
        }
    }

    public void HandleVolumeSliderChange(float volume)
    {
        SoundManager.Instance.UpdateVolume(volume);
    }

    public Toggle _uiToggle;

    public void HandleToggleInGameUI(bool value)
    {
        _uiToggle.isOn = value;
        _guiPlayHelp.HandleToggle(value);
    }

    public Toggle _pixelationToggle;

    public void HandleTogglePixelation(bool value)
    {
        _pixelationToggle.isOn = value;
    }
}