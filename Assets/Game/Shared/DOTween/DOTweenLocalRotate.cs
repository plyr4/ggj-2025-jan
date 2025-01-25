using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class TweenOpts
{
    public float _duration;
    public Ease _ease;
    public bool _loop;
    public int _loops;
    public bool _relative;
}

public class DOTweenLocalRotate : MonoBehaviour
{
    [SerializeField]
    private TweenOpts _opts;

    private Tween _tween;

    void Start()
    {
        _tween = transform.DOLocalRotate(
                new Vector3(0, 360f, 0),
                _opts._duration,
                RotateMode.FastBeyond360)
            .SetRelative(_opts._relative)
            .SetEase(_opts._ease)
            .SetLoops(_opts._loop ? _opts._loops : 0);
    }

    void OnDestroy()
    {
        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
        }
    }
}