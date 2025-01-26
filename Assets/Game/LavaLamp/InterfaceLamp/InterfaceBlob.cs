using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Osmosis : MonoBehaviour
{
    public InstancedCustomRenderTextureRenderer _finder;
    [SerializeField]
    [ReadOnlyInspector]
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
    private Tween _t;
    public float _shrinkRate = 0.02f;
    [SerializeField]
    private AnimationCurve _radiusOverLifetime;

    public RawImage _image;

    private void Start()
    {
        InstancedCustomRenderTextureRenderer.Result result = _finder.GenerateCustomRenderTextureMaterial();

        _coreMaterialInstance = result._coreMaterial;
        _image.texture = result._quadMaterial.mainTexture;

        _shaderIDs[BUBBLES_SHADER_PROPERTY] = Shader.PropertyToID(BUBBLES_SHADER_PROPERTY);
        _shaderIDs[BUBBLES2_SHADER_PROPERTY] = Shader.PropertyToID(BUBBLES2_SHADER_PROPERTY);
        _shaderIDs[BUBBLE_COUNT_SHADER_PROPERTY] = Shader.PropertyToID(BUBBLE_COUNT_SHADER_PROPERTY);
        _shaderIDs[BLOB_STATE_SHADER_PROPERTY] = Shader.PropertyToID(BLOB_STATE_SHADER_PROPERTY);

        _originalBubbleTexture = _coreMaterialInstance.GetTexture(_shaderIDs[BUBBLES_SHADER_PROPERTY]) as Texture2D;

        _bubbles.Clear();

        // // create player bubble
        Bubble b = CreateBubble(new Vector2(0f, 0f),
            0.004f, 0.004f, _radiusOverLifetime, _bubbleBaseMoveSpeed,
            true, 0f, true, false);

          b = CreateBubble(new Vector2(1f, 0f),
            0.004f, 0.004f, _radiusOverLifetime, _bubbleBaseMoveSpeed,
            true, 0f, true, false);

          b = CreateBubble(new Vector2(0f, 1f),
              0.004f, 0.004f, _radiusOverLifetime, _bubbleBaseMoveSpeed,
              true, 0f, true, false);

          b = CreateBubble(new Vector2(1f, 1f),
              0.004f, 0.004f, _radiusOverLifetime, _bubbleBaseMoveSpeed,
              true, 0f, true, false);


        b = CreateBubble(new Vector2(0.5f, 0.25f),
            0.004f, 0.004f, _radiusOverLifetime, _bubbleBaseMoveSpeed,
            true, 0f, true, false);

        // tween b up and down sine
        _t = DOTween.To(() => b._position.y, x => b._position.y = x, 0.75f, 1f).SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // playerBubble._colorID = -1;
        // _playerBubbleMono._bubble = playerBubble;
        // _playerBubbleMono.transform.localPosition = playerBubble._position;


        // initialize texture
        InitializeBubbleTextures();
        UpdateBubbleTextures();
    }

    void Update()
    {
        Bubble.UpdateOpts updateOpts = new Bubble.UpdateOpts
        {
            // _blob = this
        };
        UpdateBubbles();
        UpdateBubbleTextures();
    }

    private void OnApplicationQuit()
    {
        if (_coreMaterialInstance != null)
            _coreMaterialInstance.SetTexture(_shaderIDs[BUBBLES_SHADER_PROPERTY], _originalBubbleTexture);
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

    public void RemoveBubble(Bubble bubble)
    {
        _bubbles.Remove(bubble);
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

    public void UpdateBubbles()
    {
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