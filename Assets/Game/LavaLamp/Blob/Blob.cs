using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Blob : MonoBehaviour
{
    public InstancedCustomRenderTextureRenderer _finder;
    [SerializeField]
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
    [ReadOnlyInspector]
    public Texture2D _runtimeBubbleTexture2;
    [SerializeField]
    private bool _writeToOriginalBubbleTexture;
    // shader properties
    const string BUBBLES_SHADER_PROPERTY = "_Bubbles";
    const string BUBBLES2_SHADER_PROPERTY = "_Bubbles2";
    const string BUBBLE_COUNT_SHADER_PROPERTY = "_BubbleCount";
    const string BLOB_STATE_SHADER_PROPERTY = "_State";
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

    public float _currentState = 0.5f;
    public float _currentStateValue = 0.5f;
    public float _stateLerpSpeed = 1f;
    public bool _disabled;

    private void Start()
    {
        _quadClickHandler.OnQuadClicked += OnQuadClicked;

        Material material = new Material(_coreMaterialInstance);
        _coreMaterialInstance = material;

        _quadClickHandler.GetComponent<MeshRenderer>().material = _coreMaterialInstance;

        _shaderIDs[BUBBLES_SHADER_PROPERTY] = Shader.PropertyToID(BUBBLES_SHADER_PROPERTY);
        _shaderIDs[BUBBLES2_SHADER_PROPERTY] = Shader.PropertyToID(BUBBLES2_SHADER_PROPERTY);
        _shaderIDs[BUBBLE_COUNT_SHADER_PROPERTY] = Shader.PropertyToID(BUBBLE_COUNT_SHADER_PROPERTY);
        _shaderIDs[BLOB_STATE_SHADER_PROPERTY] = Shader.PropertyToID(BLOB_STATE_SHADER_PROPERTY);

        _originalBubbleTexture = _coreMaterialInstance.GetTexture(_shaderIDs[BUBBLES_SHADER_PROPERTY]) as Texture2D;

        _bubbles.Clear();

        // create player bubble
        Bubble playerBubble = CreateBubble(new Vector2(0.5f, 0.05f),
            _playerBubbleBaseRadius, _playerBubbleBaseRadius,
            0f, true, 0f, true, false);
        playerBubble._colorID = -1;
        _playerBubbleMono._bubble = playerBubble;
        _playerBubbleMono.transform.localPosition = playerBubble._position;

        CreateBubblePerimeter();

        // initialize texture
        InitializeBubbleTextures();
        UpdateBubbleTextures();
    }

    void Update()
    {
        _bubbles[0]._position = _playerBubbleMono.transform.localPosition;

        UpdateCurrentState();
        UpdateBubbles();
        Bubble.HandlePlayerCollisions(new Bubble.UpdateOpts
        {
            _blob = this
        });
        UpdateBubbleTextures();
    }

    void FixedUpdate()
    {
        if (_disabled) return;

        Bubble.UpdateOpts updateOpts = new Bubble.UpdateOpts
        {
            _blob = this
        };
        Bubble.HandlePlayerHeat(updateOpts);
    }

    private void OnApplicationQuit()
    {
        if (_disabled) return;

        if (_coreMaterialInstance != null)
            _coreMaterialInstance.SetTexture(_shaderIDs[BUBBLES_SHADER_PROPERTY], _originalBubbleTexture);
    }

    public void OnQuadClicked(Vector2 normalizedPos)
    {
        if (_disabled) return;

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

    private void InitializeBubbleTextures()
    {
        if (_runtimeBubbleTexture == null)
        {
            _runtimeBubbleTexture = new Texture2D(
                _bubbleTextureSize.x, _bubbleTextureSize.y,
                TextureFormat.RGBAFloat,
                false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,
            };
        }

        if (_runtimeBubbleTexture2 == null)
        {
            _runtimeBubbleTexture2 = new Texture2D(
                _bubbleTextureSize.x, _bubbleTextureSize.y,
                TextureFormat.RGBAFloat, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,
            };
        }
    }

    void UpdateBubbleTextures()
    {
        int bubbleCount = _bubbles.Count;
        int width = _runtimeBubbleTexture.width;
        int height = _runtimeBubbleTexture.height;
        Color[] bubbleData1 = new Color[width * height];
        Color[] bubbleData2 = new Color[width * height];

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                int idx = (j * height) + i;
                if (idx >= bubbleCount)
                {
                    bubbleData1[idx] = new Color(0, 0, 0, 0);
                    bubbleData2[idx] = new Color(0, 0, 0, 0);
                    continue;
                }

                Bubble bubble = _bubbles[idx];
                bubbleData1[idx] = new Color(bubble._position.x, bubble._position.y, bubble._radius, bubble._colorID);
                bubbleData2[idx] = new Color(bubble._vanity ? 1 : 0, 0, 0, 0);
                _runtimeBubbleTexture.SetPixel(i, j, bubbleData1[idx]);
                _runtimeBubbleTexture2.SetPixel(i, j, bubbleData2[idx]);
                if (_writeToOriginalBubbleTexture) _originalBubbleTexture.SetPixel(i, j, bubbleData1[idx]);
            }
        }

        // todo: only write when bubbles have changed
        _runtimeBubbleTexture.Apply();
        _runtimeBubbleTexture2.Apply();
        if (_writeToOriginalBubbleTexture) _originalBubbleTexture.Apply();
        _coreMaterialInstance.SetTexture(_shaderIDs[BUBBLES_SHADER_PROPERTY], _runtimeBubbleTexture);
        _coreMaterialInstance.SetTexture(_shaderIDs[BUBBLES2_SHADER_PROPERTY], _runtimeBubbleTexture2);
        _coreMaterialInstance.SetInt(_shaderIDs[BUBBLE_COUNT_SHADER_PROPERTY], _bubbles.Count);
    }

    public void RemoveBubble(Bubble bubble)
    {
        if (_disabled) return;

        _bubbles.Remove(bubble);
    }

    public Bubble CreateBubble(Vector2 position, float radius, float baseRadius, float moveSpeed, bool reserve,
        float lifespan,
        bool immortal,
        bool vanity)
    {
        Bubble bubble = Bubble.New(position, radius, baseRadius, _newBubbleRadiusOverLifetime, moveSpeed, lifespan,
            immortal, reserve, vanity);
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

        float ySpacing = _perimeterBubbleHeight / _perimeterBubbleCount.y;

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
                reserve, 0f, true, true);
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
                reserve, 0f, true, true);
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

    public float HandleKillBubble(Bubble bubble)
    {
        if (_disabled) return 0;

        float delta = 0.05f;
        if (bubble._colorID == 1)
        {
            delta *= -5;
        }

        float state = _currentState;
        float newState = SetCurrentStateDelta(delta);

        return _currentState;
    }

    public float SetCurrentStateDelta(float delta)
    {
        if (_disabled) return 0;

        _currentState = Mathf.Clamp01(_currentState + delta);
        return _currentState;
    }

    private void UpdateCurrentState()
    {
        if (_disabled) return;

        float value = Mathf.Lerp(_currentStateValue, _currentState, Time.deltaTime * _stateLerpSpeed);

        if (_currentStateValue - value != 0)
        {
            _currentStateValue = value;
            _coreMaterialInstance.SetFloat(_shaderIDs[BLOB_STATE_SHADER_PROPERTY], _currentStateValue);
        }
    }

    public void UpdateBubbles()
    {
        if (_disabled) return;

        List<Bubble> bubbles = _bubbles;
        for (int i = bubbles.Count - 1; i > 0; i--)
        {
            Bubble bubble = bubbles[i];
            if (bubble == null) continue;
            if (bubble._reserved) continue;

            if (bubble._killed)
            {
                bubble._goalPosition = bubbles[0]._position;
                bubble.FollowGoal(bubble._moveSpeed * 3f);
                bubble.HandleKilledRadiusUpdate(_shrinkRate);
                if (bubble._radius <= 0f)
                    RemoveBubble(bubble);
                continue;
            }

            bubble.FollowGoal(bubble._moveSpeed);
            bubble.HandleLifetimeRadiusUpdate();

            if (bubble.ShouldDie())
            {
                RemoveBubble(bubble);
            }
        }
    }
}