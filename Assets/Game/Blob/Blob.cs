using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;

// using UnityEngine.Experimental.Rendering;

[Serializable]
public class Bubble
{
    public float _radius;
    public Vector2 _position;
    public AnimationCurve _radiusOverLifetime;
    public Vector2 _goalPosition;
    public float _randomSeed;
    public bool _inCenterMass;
    public float _startTime;
    public float _lifespan;
    public bool _immortal;
    public float _baseRadius;
    public bool _replaced;
    public GameObject _replacement;
    public int _currentHealth = 1;

    public static Bubble New(Blob blob, Vector2 position, float radius, float baseRadius, float lifeSpan, bool immortal)
    {
        Bubble bubble = new Bubble();
        bubble._position = position;
        bubble._baseRadius = baseRadius;
        bubble._radius = radius;
        bubble._randomSeed = UnityEngine.Random.value;
        bubble._startTime = Time.time;
        bubble._radiusOverLifetime = blob._newBubbleRadiusOverLifetime;
        bubble._lifespan = lifeSpan;
        bubble._immortal = immortal;
        return bubble;
    }

    public float Age()
    {
        return Time.time - _startTime;
    }

    public bool ShouldDie()
    {
        return !_immortal && Age() > _lifespan && (_baseRadius *
                                                   _radiusOverLifetime.Evaluate(0)) < 0.001f;
    }

    public bool ShouldReplace()
    {
        return !_replaced && Age() > _lifespan * 0.8f;
    }

    public float TargetVisualHeight()
    {
        return 0.1f;
    }

    public void Select()
    {
        throw new NotImplementedException();
    }

    public void Deselect()
    {
        throw new NotImplementedException();
    }

    public int CurrentHealth()
    {
        return _currentHealth;
    }

    public int MaxHealth()
    {
        return 1;
    }
}

public class Blob : MonoBehaviour
{
    public InstancedCustomRenderTextureRenderer _finder;
    [SerializeField]
    [ReadOnlyInspector]
    public Material _coreMaterialInstance;
    [SerializeField]
    private QuadClickHandler _quadClickHandler;
    // quad rules
    private const float MAX_HEIGHT = 1f;
    private const float MAX_WIDTH = 1f;
    public float _massCenterBubbleRadius = 0.005f;
    public float _moveSpeed = 3.0f;
    // bubbles
    public int _centerMassCreateBubbleCount = 25;
    private int _numReservedBubbles;
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
    private float _maxRadius = 0.0001f;
    [SerializeField]
    private float _radiusSpeed = 2f;
    [SerializeField]
    private float _radiusAmplitude = 0.18f;
    [SerializeField]
    private float _orbitOffset = 50f;
    [SerializeField]
    private float _orbitSpeed = 0.1f;
    [SerializeField]
    private Vector2 _massPosition = Vector2.one * 0.5f;
    [SerializeField]
    public AnimationCurve _newBubbleRadiusOverLifetime;
    // [SerializeField]
    // private float _newBubbleBaseRadius = 0.004f;
    [SerializeField]
    private float _massRadius;
    public GameObject _face;
    [SerializeField]
    private Vector3 _massCenterFacePositionOffset;
    [SerializeField]
    private float _faceAveragePositionFactor = 0.35f;
    [SerializeField]
    private GameObject _replacementBubblePrefab;
    [SerializeField]
    private GameObject _newObjectsParent;

    private void OnApplicationQuit()
    {
        if (_coreMaterialInstance != null)
            _coreMaterialInstance.SetTexture(_shaderIDs[BUBBLES_SHADER_PROPERTY], _originalBubbleTexture);
    }

    // public Bubble SpawnBubbleOnTile(HexTile tile)
    // {
    //     Bubble bubble = CreateBubble(_massPosition, 0f, _newBubbleBaseRadius, false, 2f, false);
    //     Vector2 pos = _quadClickHandler.GetQuadNormalizedPosition(tile._worldPosition);
    //     bubble._goalPosition = pos;
    //     bubble._tile = tile;
    //     return bubble;
    // }
    
    private void Start()
    {
        InstancedCustomRenderTextureRenderer.Result result = _finder.GenerateCustomRenderTextureMaterial();

        _coreMaterialInstance = result._coreMaterial;
        _quadClickHandler.GetComponent<MeshRenderer>().material = result._quadMaterial;

        _shaderIDs[BUBBLES_SHADER_PROPERTY] = Shader.PropertyToID(BUBBLES_SHADER_PROPERTY);
        _shaderIDs[BUBBLE_COUNT_SHADER_PROPERTY] = Shader.PropertyToID(BUBBLE_COUNT_SHADER_PROPERTY);

        _originalBubbleTexture = _coreMaterialInstance.GetTexture(_shaderIDs[BUBBLES_SHADER_PROPERTY]) as Texture2D;

        _bubbles.Clear();

        // center bubble
        Bubble b = CreateBubble(_massPosition, _massCenterBubbleRadius, _massCenterBubbleRadius, true, 0f, true);
        b._inCenterMass = true;

        // the rest of the evil mass
        CreateBubbles();

        // initialize texture
        InitializeBubbleTexture();
        UpdateBubbleTexture();
    }

    void Update()
    {
        // MovePlayer();
        UpdateBubbles();
        UpdateBubbleTexture();
    }

    private Bubble CreateBubble(Vector2 position, float radius, float baseRadius, bool reserve, float lifespan,
        bool immortal)
    {
        Bubble bubble = Bubble.New(this, position, radius, baseRadius, lifespan, immortal);
        _bubbles.Add(bubble);
        if (reserve) _numReservedBubbles++;
        return bubble;
    }

    void CreateBubbles()
    {
        for (int i = _numReservedBubbles; i < _centerMassCreateBubbleCount; i++)
        {
            Vector2 pos = RandomBubblePositionForCenterMass();
            float radius = RandomBubbleRadiusForCenterMass();
            Bubble bubble = CreateBubble(pos, radius, radius, false, 0f, true);
            bubble._inCenterMass = true;
        }
    }

    private Vector2 RandomBubblePositionForCenterMass()
    {
        return new Vector2(0.5f + UnityEngine.Random.Range(-MAX_WIDTH / 1f, MAX_WIDTH / 1f),
            0.5f + UnityEngine.Random.Range(-MAX_HEIGHT / 1f, MAX_HEIGHT / 1f));
    }

    private float RandomBubbleRadiusForCenterMass()
    {
        return Mathf.Clamp(UnityEngine.Random.Range(_maxRadius / 5f, _maxRadius / 3f), 0.001f, _maxRadius);
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
                bubbleData[idx] = new Color(bubble._position.x, bubble._position.y, bubble._radius, 0);
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

    void UpdateBubbles()
    {
        _bubbles[0]._position = _massPosition;
        Vector2 averagePosition = Vector2.zero;
        int bubblesInMass = 0;
        for (int i = _bubbles.Count - 1; i > _numReservedBubbles - 1; i--)
        {
            Bubble bubble = _bubbles[i];
            if (bubble == null) continue;
            if (!bubble._inCenterMass)
            {
                bubble._position =
                    Vector2.Lerp(bubble._position, bubble._goalPosition, _moveSpeed * Time.deltaTime);

                if (Vector2.Distance(bubble._position, bubble._goalPosition) < 0.001f)
                {
                    bubble._position = bubble._goalPosition;
                }

                float radius = bubble._baseRadius *
                               bubble._radiusOverLifetime.Evaluate((bubble.Age() / bubble._lifespan));

                bubble._radius = radius;

                if (bubble.ShouldReplace())
                {
                    // Vector3 pos = bubble._tile._worldPosition;
                    // GameObject replacement = Instantiate(_replacementBubblePrefab, pos, Quaternion.identity,
                    //     _newObjectsParent.transform);
                    // bubble._replaced = true;
                    // bubble._replacement = replacement;
                    // replacement.transform.localScale = Vector3.one * 1.8f;
                }

                if (bubble.ShouldDie())
                {
                    RemoveBubble(bubble);
                }

                continue;
            }

            bubble._radius = bubble._baseRadius * (1 + _massRadius);

            float dynamicRadius = bubble._baseRadius +
                                  Mathf.Sin(Time.time * _radiusSpeed * bubble._randomSeed) * _radiusAmplitude *
                                  (1 + _massRadius);
            float angle = Time.time * _orbitSpeed + i * _orbitOffset;
            bubble._position = _massPosition + new Vector2(
                Mathf.Cos(angle) * dynamicRadius,
                Mathf.Sin(angle) * dynamicRadius
            );
            averagePosition += (bubble._position - _massPosition);
            bubblesInMass++;
        }

        averagePosition /= bubblesInMass;

        UpdateFacePosition(averagePosition);
    }

    private void RemoveBubble(Bubble bubble)
    {
        _bubbles.Remove(bubble);
    }

    private void UpdateFacePosition(Vector2 averagePosition)
    {
        _face.transform.localPosition = _massPosition - Vector2.one * 0.5f;
        _face.transform.localPosition += _massCenterFacePositionOffset;
        _face.transform.localPosition +=
            new Vector3(averagePosition.x, averagePosition.y, 0) * _faceAveragePositionFactor;
    }
}