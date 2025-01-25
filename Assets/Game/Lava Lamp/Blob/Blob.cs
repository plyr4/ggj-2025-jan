using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Blob : MonoBehaviour
{
    public InstancedCustomRenderTextureRenderer _finder;
    [SerializeField]
    [ReadOnlyInspector]
    public Material _coreMaterialInstance;
    [SerializeField]
    private QuadClickHandler _quadClickHandler;

    private Vector2Int _bubbleTextureSize = new Vector2Int(32, 32);
    public List<Bubble> _bubbles = new List<Bubble>();
    public Texture2D _originalBubbleTexture;
    [SerializeField]
    [ReadOnlyInspector]
    public Texture2D _runtimeBubbleTexture;
    [SerializeField]
    private bool _writeToOriginalBubbleTexture;
    // shader properties
    const string BUBBLES_SHADER_PROPERTY = "_Bubbles";
    const string BUBBLE_COUNT_SHADER_PROPERTY = "_BubbleCount";
    private readonly Dictionary<string, int> _shaderIDs = new Dictionary<string, int>();

    [SerializeField]
    public AnimationCurve _newBubbleRadiusOverLifetime;
    [SerializeField]
    public float _newBubbleBaseRadius = 0.002f;
    [SerializeField]
    private float _massRadius;
    [SerializeField]
    private Vector3 _massCenterFacePositionOffset;
    public float _bubbleBaseMoveSpeed = 0.1f;
    public float _distanceCheckFactor = 5f;
    public float _shrinkRate = 0.02f;
    public float _mergeGrowthRate = 0.05f;

    public Vector2Int _perimeterBubbleCount = new Vector2Int(10, 4);
    public float _perimeterSideBubbleRadius = 0.002f;
    [Range(0, 1)]
    public float _perimeterBubbleHeight = 0.3f;
    public float _sideHeightIncreaseFactor = 0.00005f;
    public float _sidePadding;
    public float _playerBubbleBaseMoveSpeed = 0.5f;
    private Tween _t;
    public List<HeatSource> _heatSources;
    [Space]
    [SerializeField]
    public float _playerBubbleBaseRadius = 0.002f;
    public BubbleMono _playerBubbleMono;

    private void Start()
    {
        _quadClickHandler.OnQuadClicked += OnQuadClicked;

        InstancedCustomRenderTextureRenderer.Result result = _finder.GenerateCustomRenderTextureMaterial();

        _coreMaterialInstance = result._coreMaterial;
        _quadClickHandler.GetComponent<MeshRenderer>().material = result._quadMaterial;

        _shaderIDs[BUBBLES_SHADER_PROPERTY] = Shader.PropertyToID(BUBBLES_SHADER_PROPERTY);
        _shaderIDs[BUBBLE_COUNT_SHADER_PROPERTY] = Shader.PropertyToID(BUBBLE_COUNT_SHADER_PROPERTY);

        _originalBubbleTexture = _coreMaterialInstance.GetTexture(_shaderIDs[BUBBLES_SHADER_PROPERTY]) as Texture2D;

        _bubbles.Clear();

        // create player bubble
        Bubble playerBubble = CreateBubble(new Vector2(0.5f, 0.05f),
            _playerBubbleBaseRadius, _playerBubbleBaseRadius,
            0f, true, 0f, true);
        playerBubble._colorID = -1;
        _playerBubbleMono._bubble = playerBubble;

        CreateBubblePerimeter();

        // initialize texture
        InitializeBubbleTexture();
        UpdateBubbleTexture();
    }

    void Update()
    {
        _bubbles[0]._position = _playerBubbleMono.transform.localPosition;

        Bubble.UpdateOpts updateOpts = new Bubble.UpdateOpts
        {
            _blob = this
        };
        Bubble.UpdateBubbles(updateOpts);
        Bubble.HandlePlayerCollisions(updateOpts);
        UpdateBubbleTexture();
    }

    void FixedUpdate()
    {
        Bubble.UpdateOpts updateOpts = new Bubble.UpdateOpts
        {
            _blob = this
        };
        Bubble.HandlePlayerHeat(updateOpts);
    }

    private void OnApplicationQuit()
    {
        if (_coreMaterialInstance != null)
            _coreMaterialInstance.SetTexture(_shaderIDs[BUBBLES_SHADER_PROPERTY], _originalBubbleTexture);
    }

    public void OnQuadClicked(Vector2 normalizedPos)
    {
        Bubble bubble = _bubbles[0];
        if (_t != null)
        {
            _t.Kill();
            _t = null;
        }

        // duration based on distance
        float d = Vector2.Distance(bubble._position, normalizedPos) *
            1 / Mathf.Max(_playerBubbleBaseMoveSpeed, 0.1f);

        _t = DOTween.To(
                () => bubble._position,
                x => bubble._position = x,
                normalizedPos,
                d)
            .SetEase(Ease.InOutSine);
    }

    public Bubble CreateBubble(Vector2 position, float radius, float baseRadius, float moveSpeed, bool reserve,
        float lifespan,
        bool immortal)
    {
        Bubble bubble = Bubble.New(this, position, radius, baseRadius, moveSpeed, lifespan, immortal, reserve);
        if (reserve)
        {
            int numReservedBubbles = 0;
            for (int i = 0; i < _bubbles.Count; i++)
            {
                if (_bubbles[i]._reserved)
                {
                    numReservedBubbles += 1;
                }
            }

            _bubbles.Insert(numReservedBubbles, bubble);
        }
        else
        {
            _bubbles.Add(bubble);
        }


        return bubble;
    }

    void CreateBubblePerimeter()
    {
        bool reserve = true;

        float xSpacing = 1f / _perimeterBubbleCount.x;
        float ySpacing = _perimeterBubbleHeight / _perimeterBubbleCount.y;

        for (int x = 0; x <= _perimeterBubbleCount.x; x += 1)
        {
            // float yDistanceToWiggle = 0.075f;
            // float duration = 3f;
            // yDistanceToWiggle += UnityEngine.Random.Range(0f, 0.02f);
            // duration += UnityEngine.Random.Range(-1f, 1f);
            // float randomDelay = UnityEngine.Random.Range(0f, 1f);
            //
            // float r = _perimeterBottomBubbleRadius;
            // r += UnityEngine.Random.Range(-_bottomBubbleRandomRadius, _bottomBubbleRandomRadius);
            //
            // Bubble b1 = CreateBubble(new Vector2(x * xSpacing, -yDistanceToWiggle / 2f), r, r,
            //     _bubbleBaseMoveSpeed,
            //     reserve, 0f, true);
            //
            // DOTween.To(
            //         () => b1._position,
            //         x => b1._position = x,
            //         new Vector2(0, yDistanceToWiggle),
            //         duration)
            //     .SetDelay(randomDelay)
            //     .SetLoops(-1, LoopType.Yoyo)
            //     .SetEase(Ease.InOutSine)
            //     .SetRelative(true);
        }

        for (int y = 0; y < _perimeterBubbleCount.y; y += 1)
        {
            float yDistanceToWiggle = 0.08f;
            yDistanceToWiggle += UnityEngine.Random.Range(0f, 0.02f);

            float duration = 3f;
            duration += UnityEngine.Random.Range(-1f, 1f);

            float r = _perimeterSideBubbleRadius + UnityEngine.Random.Range(0f, _perimeterSideBubbleRadius / 4f);

            r += (_perimeterBubbleCount.y - y) * _sideHeightIncreaseFactor;

            Bubble b1 = CreateBubble(new Vector2(-_sidePadding, y * ySpacing), r, r,
                _bubbleBaseMoveSpeed,
                reserve, 0f, true);
            DOTween.To(
                    () => b1._position,
                    x => b1._position = x,
                    new Vector2(0, yDistanceToWiggle),
                    duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetRelative(true);

            yDistanceToWiggle = 0.08f;
            yDistanceToWiggle += UnityEngine.Random.Range(0f, 0.02f);

            duration = 3f;
            duration += UnityEngine.Random.Range(-1f, 1f);

            Bubble b2 = CreateBubble(new Vector2(1 + _sidePadding, y * ySpacing), r, r,
                _bubbleBaseMoveSpeed,
                reserve, 0f, true);
            DOTween.To(
                    () => b2._position,
                    x => b2._position = x,
                    new Vector2(0, yDistanceToWiggle),
                    duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetRelative(true);
        }
    }

    private void InitializeBubbleTexture()
    {
        if (_runtimeBubbleTexture == null)
        {
            _runtimeBubbleTexture = new Texture2D(
                _bubbleTextureSize.x, _bubbleTextureSize.y,
                TextureFormat.RGBAFloat, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,
            };
        }
    }

    void UpdateBubbleTexture()
    {
        int bubbleCount = _bubbles.Count;
        int width = _runtimeBubbleTexture.width;
        int height = _runtimeBubbleTexture.height;
        Color[] bubbleData = new Color[width * height];

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                int idx = (j * height) + i;
                if (idx >= bubbleCount)
                {
                    bubbleData[idx] = new Color(0, 0, 0, 0);
                    continue;
                }

                Bubble bubble = _bubbles[idx];
                bubbleData[idx] = new Color(bubble._position.x, bubble._position.y, bubble._radius, bubble._colorID);
                _runtimeBubbleTexture.SetPixel(i, j, bubbleData[idx]);
                if (_writeToOriginalBubbleTexture) _originalBubbleTexture.SetPixel(i, j, bubbleData[idx]);
            }
        }

        // todo: only write when bubbles have changed
        _runtimeBubbleTexture.Apply();
        if (_writeToOriginalBubbleTexture) _originalBubbleTexture.Apply();
        _coreMaterialInstance.SetTexture(_shaderIDs[BUBBLES_SHADER_PROPERTY], _runtimeBubbleTexture);
        _coreMaterialInstance.SetInt(_shaderIDs[BUBBLE_COUNT_SHADER_PROPERTY], _bubbles.Count);
    }

    public void RemoveBubble(Bubble bubble)
    {
        _bubbles.Remove(bubble);
    }
}