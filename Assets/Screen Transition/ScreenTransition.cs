using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTransition : MonoBehaviour
{
    private static ScreenTransition _instance;
    public static ScreenTransition Instance
    {
        get
        {
            // attempt to locate the singleton
            if (_instance == null)
            {
                _instance = (ScreenTransition)FindObjectOfType(typeof(ScreenTransition));
            }

            // create a new singleton
            if (_instance == null)
            {
                _instance = (new GameObject("Screen Transition")).AddComponent<ScreenTransition>();
            }

            // return singleton
            return _instance;
        }
    }

    [SerializeField]
    private RenderType _renderType = RenderType.ImageMaterial;
    [SerializeField]
    private Material _mat;
    [SerializeField]
    private float _transitionSpeed = 2f;
    [SerializeField]
    [ReadOnlyInspector]
    private float _transitionProgress = 0f;
    [SerializeField]
    [ReadOnlyInspector]
    private TransitionState _state = 0f;
    [SerializeField]
    private GameEvent _onTransitionOpenDoneEvent;
    [SerializeField]
    private GameEvent _onTransitionCloseDoneEvent;
    [SerializeField]
    private RawImage _image;
    [SerializeField]
    private GameObject _scaler;
    [SerializeField]
    [ReadOnlyInspector]
    private bool _transitioning;

    public enum TransitionState
    {
        Close,
        Open
    }

    public enum RenderType
    {
        ImageMaterial,
        Scaler
    }

    void Start()
    {
        _transitionProgress = 1f;
        _state = TransitionState.Close;
        _transitioning = false;

        switch (_renderType)
        {
            case RenderType.ImageMaterial:
                string matName = _mat.name;
                _mat = new Material(_mat);
                _mat.name = matName + " (Instance)";
                _image.material = _mat;
                break;
            case RenderType.Scaler:
                break;
        }
    }

    void Update()
    {
        switch (_state)
        {
            case TransitionState.Close:
                _transitionProgress += Time.deltaTime * _transitionSpeed;
                _transitionProgress = Mathf.Clamp(_transitionProgress, 0f, 1f);

                if (_transitionProgress >= 1f)
                {
                    if (_transitioning)
                    {
                        _transitioning = false;
                        _onTransitionCloseDoneEvent.Invoke();
                    }
                }

                break;
            case TransitionState.Open:
                _transitionProgress -= Time.deltaTime * _transitionSpeed;
                _transitionProgress = Mathf.Clamp(_transitionProgress, 0f, 1f);

                if (_transitionProgress <= 0f)
                {
                    if (_transitioning)
                    {
                        _transitioning = false;
                        _onTransitionOpenDoneEvent.Invoke();
                    }
                }

                break;
        }

        switch (_renderType)
        {
            case RenderType.ImageMaterial:
                _mat.SetFloat("_Progress", _transitionProgress);
                break;
            case RenderType.Scaler:
                float p = Mathf.Clamp(_transitionProgress, 0f, .999f);
                _scaler.transform.localScale = Vector3.one * (1f - p);
                break;
        }
    }

    public void CloseImmediate()
    {
        _state = TransitionState.Close;
        _transitioning = false;
        _transitionProgress = 1f;
    }

    public void OpenImmediate()
    {
        _state = TransitionState.Open;
        _transitioning = true;
        _transitionProgress = 0f;
    }

    public void Close()
    {
        _state = TransitionState.Close;
        _transitioning = true;
    }

    public void Open(float delay)
    {
        StartCoroutine(delayedOpen(delay));
    }

    IEnumerator delayedOpen(float delay)
    {
        yield return new WaitForSeconds(delay);
        _state = TransitionState.Open;
        _transitioning = true;
    }
}