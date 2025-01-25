using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play : MonoBehaviour
{
    [SerializeField]
    private GameEvent _playPause;
    [SerializeField]
    private GameEvent _pausePlay;
    [SerializeField]
    private GStates _receiveInputGameStateMask;

    // button cooldown
    private bool _buttonCooldown;
    private float _buttonCooldownDuration = 0f;

    public void Update()
    {
        bool state = GStateMachineGame.Instance.ContainsCurrentState(_receiveInputGameStateMask.ToList());
        if (!state) return;
        bool input = GameInput.Instance._menuBackPressed;
        if (!_buttonCooldown && input)
        {
            StartCoroutine(buttonCooldown());
            switch (GStateMachineGame.Instance.CurrentState())
            {
                case GStatePlay:
                    _playPause.Invoke();
                    break;
                case GStatePause:
                    _pausePlay.Invoke();
                    break;
            }
        }
    }

    private IEnumerator buttonCooldown()
    {
        _buttonCooldown = true;
        yield return new WaitForSeconds(_buttonCooldownDuration);
        _buttonCooldown = false;
    }
}