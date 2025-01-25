using System.Collections;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class GUIDollars : MonoBehaviour
{
    public TextMeshProUGUI _text;
    // private string _extra = "";
    private Tween _tween;

    private void Update()
    {
        // int dollars = Run.Instance._dollars;
        // _text.text = $"${dollars}{_extra}";
        _text.transform.localScale = Vector3.Lerp(_text.transform.localScale, Vector3.one, Time.deltaTime * 3f);
    }

    public void HandleEarnMoney(IGameEventOpts opts)
    {
        StopAllCoroutines();
        StartCoroutine(AddMoney());
    }

    IEnumerator AddMoney()
    {
        // _extra = "<color=green>+</color>";
        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
        }

        // out bounce out quad local scale juice
        _tween = _text.transform.DOPunchScale(Vector3.one * 0.1f, 0.25f, 5, 0.5f);
        yield return new WaitForSeconds(0.25f);
        // _extra = "";
    }
}