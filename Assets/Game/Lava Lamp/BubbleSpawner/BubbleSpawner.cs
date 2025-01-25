using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class BubbleSpawner : MonoBehaviour
{
    [Serializable]
    public class SpawnOpts
    {
        public float _spawnDelay = 0.5f;
        public float _lifespan = 10f;
        public float _radius = 1f;
        public float _moveSpeed = 1f;
        public float _vanityTweenTimeScale = 1f;

        public SpawnOpts()
        {
        }

        public SpawnOpts(SpawnOpts opts)
        {
            _spawnDelay = opts._spawnDelay;
            _lifespan = opts._lifespan;
            _radius = opts._radius;
            _moveSpeed = opts._moveSpeed;
            _vanityTweenTimeScale = opts._vanityTweenTimeScale;
        }
    }

    public Blob _blob;
    public List<Bubble> _bubbles = new List<Bubble>();
    public bool _isHeating;
    [SerializeField]
    public SpawnOpts _baseSpawnOpts;
    [SerializeField]
    private SpawnOpts _heatingOpts;
    [SerializeField]
    private SpawnOpts _runtimeSpawnOpts = new SpawnOpts();
    [SerializeField]
    private float _lerpSpeed = 1f;
    [SerializeField]
    [ReadOnlyInspector]
    private float _optsValue = 0f;

    [Space]
    public bool _debug = false;
    public MeshRenderer _debugMeshRenderer;

    public void SetHeating(bool heating)
    {
        _isHeating = heating;
    }

    private void FixedUpdate()
    {
        SetHeating(false);

        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity);
        foreach (Collider collider in colliders)
        {
            HeatSourceCollider heatSource = collider.GetComponent<HeatSourceCollider>();
            if (heatSource != null)
            {
                SetHeating(true);
            }
        }
    }

    private void Update()
    {
        if (_isHeating)
        {
            _debugMeshRenderer.material.color = Color.red;
            _optsValue += Time.deltaTime * _lerpSpeed;
        }
        else
        {
            _debugMeshRenderer.material.color = Color.white;
            _optsValue -= Time.deltaTime * _lerpSpeed;
        }

        _optsValue = Mathf.Clamp01(_optsValue);

        _runtimeSpawnOpts._spawnDelay = Mathf.Lerp(_baseSpawnOpts._spawnDelay, _heatingOpts._spawnDelay, _optsValue);
        _runtimeSpawnOpts._lifespan = Mathf.Lerp(_baseSpawnOpts._lifespan, _heatingOpts._lifespan, _optsValue);
        _runtimeSpawnOpts._radius = Mathf.Lerp(_baseSpawnOpts._radius, _heatingOpts._radius, _optsValue);
        _runtimeSpawnOpts._vanityTweenTimeScale = Mathf.Lerp(_baseSpawnOpts._vanityTweenTimeScale,
            _heatingOpts._vanityTweenTimeScale, _optsValue);

        _runtimeSpawnOpts._moveSpeed = Mathf.Lerp(_baseSpawnOpts._moveSpeed, _heatingOpts._moveSpeed, _optsValue);

        // update all bubbles speed
        foreach (Bubble bubble in _bubbles)
        {
            float y = 1 - bubble._position.y;
            float diff = _runtimeSpawnOpts._moveSpeed - _baseSpawnOpts._moveSpeed;
            bubble._moveSpeed = _baseSpawnOpts._moveSpeed + diff * y * y;
        }

        // update tweens time scale
        foreach (Tween tween in _tweens)
        {
            tween.timeScale = _runtimeSpawnOpts._vanityTweenTimeScale;
        }
    }

    private IEnumerator Start()
    {
        if (!_debug) _debugMeshRenderer.enabled = false;

        _runtimeSpawnOpts = new SpawnOpts(_baseSpawnOpts);
        SpawnVanityBubbles();

        yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 3f));

        StartSpawning();
    }

    public float _perimeterBottomBubbleRadius = 0.002f;
    public float _bottomBubbleRandomRadius = 0.001f;
    public float _bubbleBaseMoveSpeed = 0.1f;

    private List<Tween> _tweens = new List<Tween>();

    private void SpawnVanityBubbles()
    {
        foreach (Tween tween in _tweens)
        {
            tween.Kill();
        }

        _tweens.Clear();
        float yDistanceToWiggle = 0.05f;
        yDistanceToWiggle += UnityEngine.Random.Range(0f, 0.02f);
        float duration = 3f;
        duration += UnityEngine.Random.Range(-1f, 1f);
        float randomDelay = UnityEngine.Random.Range(0f, 1f);

        float r = _perimeterBottomBubbleRadius;
        r += UnityEngine.Random.Range(_bottomBubbleRandomRadius/2, _bottomBubbleRandomRadius *1.5f);
        float x = 0.03f;
        x += UnityEngine.Random.Range(-0.01f, 0.01f);

        Bubble b1 = _blob.CreateBubble(new Vector2(transform.localPosition.x - x,
                transform.localPosition.y),
            r, r,
            _bubbleBaseMoveSpeed,
            true, 0f, true);

        Tween t = DOTween.To(
                () => b1._position,
                x => b1._position = x,
                new Vector2(0, yDistanceToWiggle),
                duration)
            .SetDelay(randomDelay)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetRelative(true);

        _tweens.Add(t);

        r += UnityEngine.Random.Range(-r / 2, r / 2);

        Bubble b2 = _blob.CreateBubble(new Vector2(transform.localPosition.x + x,
                transform.localPosition.y),
            r, r,
            _bubbleBaseMoveSpeed,
            true, 0f, true);
        duration += UnityEngine.Random.Range(-1f, 1f);
        randomDelay += UnityEngine.Random.Range(-0.5f, 1f);
        t = DOTween.To(
                () => b2._position,
                x => b2._position = x,
                new Vector2(0, yDistanceToWiggle),
                duration)
            .SetDelay(randomDelay)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetRelative(true);

        _tweens.Add(t);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        foreach (Tween tween in _tweens)
        {
            tween.Kill();
        }
    }

    private void StartSpawning()
    {
        StartCoroutine(spawnBubble());
    }

    private IEnumerator spawnBubble()
    {
        float d = UnityEngine.Random.Range(0f, 0.5f);
        yield return new WaitForSeconds(_runtimeSpawnOpts._spawnDelay + d);

        Vector2 startingPos = transform.localPosition;
        // add some perlin noise
        startingPos.x += Mathf.PerlinNoise(Time.time, 0f) * 0.1f;
        float endHeight = 1.5f;
        float r = _blob._newBubbleBaseRadius * _runtimeSpawnOpts._radius;
        r += UnityEngine.Random.Range(_blob._newBubbleBaseRadius, _blob._newBubbleBaseRadius * 2);
        // very rarely increase the size by 1.5x
        if (UnityEngine.Random.value < 0.1f)
        {
            r *= 1.5f;
        }

        float speed = _blob._bubbleBaseMoveSpeed * _runtimeSpawnOpts._moveSpeed;
        float lifespan = _runtimeSpawnOpts._lifespan + UnityEngine.Random.Range(0f, 3f);

        Bubble bubble = _blob.CreateBubble(startingPos, r, r, speed, false, lifespan, false);
        bubble._goalPosition = new Vector3(startingPos.x, endHeight);

        _bubbles.Add(bubble);

        StartCoroutine(spawnBubble());
    }
}