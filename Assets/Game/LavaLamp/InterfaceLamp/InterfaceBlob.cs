using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceBlob : MonoBehaviour
{
    [SerializeField]
    public Material _coreMaterialInstance;
    private Vector2Int _bubbleTextureSize = new Vector2Int(32, 32);
    public List<Bubble> _bubbles = new List<Bubble>();
    public Texture2D _originalBubbleTexture;
    [SerializeField]
    private bool _writeToOriginalBubbleTexture;
    [SerializeField]
    [ReadOnlyInspector]
    public Texture2D _runtimeBubbleTexture;
    [SerializeField]
    [ReadOnlyInspector]
    public Texture2D _runtimeBubbleTexture2;
    // shader properties
    const string BUBBLES_SHADER_PROPERTY = "_Bubbles";
    const string BUBBLES2_SHADER_PROPERTY = "_Bubbles2";
    const string BUBBLE_COUNT_SHADER_PROPERTY = "_BubbleCount";
    const string BLOB_STATE_SHADER_PROPERTY = "_State";
    private readonly Dictionary<string, int> _shaderIDs = new Dictionary<string, int>();

    [SerializeField]
    public float _bubbleBaseMoveSpeed = 0.1f;
    [Range(0, 1)]
    public float _perimeterBubbleHeight = 0.3f;
    private List<Tween> _tweens = new List<Tween>();
    public float _shrinkRate = 0.02f;
    [SerializeField]
    private AnimationCurve _radiusOverLifetime;
    public Image _image;
    public Vector2 _center = new Vector2(0.5f, 0.5f);
    public float _yHeight;
    public Vector2 _bubbleOffset = new Vector2(0f, 0f);
    public float _animationSpeed = 2f;

    private void Awake()
    {
        Material material = new Material(_coreMaterialInstance);
        _coreMaterialInstance = material;

        if (_image == null)
        {
            _image = GetComponent<Image>();
        }

        _image.material = _coreMaterialInstance;

        _shaderIDs[BUBBLES_SHADER_PROPERTY] = Shader.PropertyToID(BUBBLES_SHADER_PROPERTY);
        _shaderIDs[BUBBLES2_SHADER_PROPERTY] = Shader.PropertyToID(BUBBLES2_SHADER_PROPERTY);
        _shaderIDs[BUBBLE_COUNT_SHADER_PROPERTY] = Shader.PropertyToID(BUBBLE_COUNT_SHADER_PROPERTY);
        _shaderIDs[BLOB_STATE_SHADER_PROPERTY] = Shader.PropertyToID(BLOB_STATE_SHADER_PROPERTY);

        _originalBubbleTexture = _coreMaterialInstance.GetTexture(_shaderIDs[BUBBLES_SHADER_PROPERTY]) as Texture2D;

        _bubbles.Clear();

        if (_tweens != null)
        {
            foreach (Tween tween in _tweens)
            {
                tween.Kill();
            }
        }

        Bubble b1 = CreateBubble(_center + _bubbleOffset - new Vector2(0.07f, 0f),
            0.006f, 0.006f, _radiusOverLifetime, _bubbleBaseMoveSpeed,
            true, 0f, true, false);
        Tween t1 = DOTween.To(() => b1._position.y, x => b1._position.y = x, _yHeight, _animationSpeed)
            .SetEase(Ease.InOutSine)
            .SetRelative(true)
            .SetUpdate(true)
            .SetLoops(-1, LoopType.Yoyo);
        _tweens.Add(t1);
        b1._colorID = 0;


        Bubble b2 = CreateBubble(_center + _bubbleOffset + new Vector2(0.07f, 0f),
            0.007f, 0.007f, _radiusOverLifetime, _bubbleBaseMoveSpeed,
            true, 0f, true, false);
        Tween t2 = DOTween.To(() => b2._position.y, x => b2._position.y = x, _yHeight, 2f * _animationSpeed)
            .SetEase(Ease.OutSine)
            .SetRelative(true)
            .SetUpdate(true)
            .SetLoops(-1, LoopType.Yoyo);
        _tweens.Add(t2);
        b2._colorID = 0;


        Bubble b3 = CreateBubble(_center,
            0.005f, 0.0025f, _radiusOverLifetime, _bubbleBaseMoveSpeed,
            true, 0f, true, false);
        Tween t3 = DOTween.To(() => b3._position.y, x => b3._position.y = x, 0.1f, _animationSpeed)
            .SetEase(Ease.InOutSine)
            .SetRelative(true)
            .SetUpdate(true)
            .SetLoops(-1, LoopType.Yoyo);
        _tweens.Add(t3);
        b3._colorID = -1;

        Bubble b4 = CreateBubble(_center + _bubbleOffset + new Vector2(0.055f, 0f),
            0.004f, 0.004f, _radiusOverLifetime, _bubbleBaseMoveSpeed,
            true, 0f, true, false);
        Tween t4 = DOTween.To(() => b4._position.y, x => b4._position.y = x, _yHeight * 2.5f, 4f * _animationSpeed)
            .SetEase(Ease.InOutSine)
            .SetRelative(true)
            .SetUpdate(true)
            .SetLoops(-1, LoopType.Yoyo);
        _tweens.Add(t4);
        b4._colorID = 0;

        // initialize texture
        InitializeBubbleTextures();
        UpdateBubbleTextures();
    }

    void Update()
    {
        UpdateBubbleTextures();
    }

    private void InitializeBubbleTextures()
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

    public Bubble CreateBubble(Vector2 position, float radius, float baseRadius, AnimationCurve radiusOverLifetime,
        float moveSpeed, bool reserve,
        float lifespan,
        bool immortal,
        bool vanity)
    {
        Bubble bubble = Bubble.New(position, radius, baseRadius, radiusOverLifetime, moveSpeed, lifespan, immortal,
            reserve, vanity);
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

    public void UpdateState(float state)
    {
        _coreMaterialInstance.SetFloat(_shaderIDs[BLOB_STATE_SHADER_PROPERTY], state);
    }
}