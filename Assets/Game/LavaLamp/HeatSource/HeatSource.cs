using UnityEngine;

public class HeatSource : MonoBehaviour
{
    private Vector2 _bounds = new Vector2(-0.5f, 0.5f);
    public float _speed = 2f;
    public GameObject _quadScaler;
    public Vector2 _normalizedPosition;
    public float _normalizedHeight = -0.1f;
    [SerializeField]
    [ReadOnlyInspector]
    public Vector2 _normalizedWidthBounds;

    public Mode _mode = Mode.Horizontal;
    public float _collisionPadding = 0.05f;

    public enum Mode
    {
        Vertical_Left,
        Vertical_Right,
        Horizontal
    }

    void Start()
    {
        transform.position = new Vector3((_bounds.x + _bounds.y) / 2f, transform.position.y, transform.position.z);
    }

    void Update()
    {
        _normalizedWidthBounds = GetNormalizedWidthBounds();
        Vector3 movement = Vector3.zero;
        switch (_mode)
        {
            case Mode.Horizontal:
                movement = GameInput.Instance._horizontalMovementHeatSource;
                movement.y = 0;
                break;
            case Mode.Vertical_Left:
                movement = GameInput.Instance._verticalMovementHeatSourceLeft;
                movement.x = -movement.y;
                movement.y = 0;
                break;
            case Mode.Vertical_Right:
                movement = GameInput.Instance._verticalMovementHeatSourceRight;
                movement.x = movement.y;
                movement.y = 0;
                break;
        }

        if (movement != Vector3.zero)
        {
            transform.localPosition += movement * Time.deltaTime * _speed;
            Vector2 xBounds = _bounds;
            xBounds.x += _quadScaler.transform.localScale.x / 2;
            xBounds.y -= _quadScaler.transform.localScale.x / 2;
            transform.localPosition = new Vector3(
                Mathf.Clamp(transform.localPosition.x, xBounds.x, xBounds.y),
                transform.localPosition.y,
                transform.localPosition.z
            );
        }

        _normalizedPosition = GetNormalizedPosition();
    }

    public float QuadHorizontalWidth()
    {
        return _quadScaler.transform.localScale.x;
    }

    public Vector2 GetNormalizedWidthBounds()
    {
        Vector2 p = new Vector2(
            (transform.localPosition.x - QuadHorizontalWidth() / 2 - _bounds.x) /
            (_bounds.y - _bounds.x),
            (transform.localPosition.x + QuadHorizontalWidth() / 2 - _bounds.x) /
            (_bounds.y - _bounds.x)
        );

        Vector3 verticalLocalPosition = transform.localPosition;
        switch (_mode)
        {
            case Mode.Vertical_Left:
                verticalLocalPosition.x = -verticalLocalPosition.x;
                p = new Vector2(
                    (verticalLocalPosition.x - QuadHorizontalWidth() / 2 - _bounds.x) /
                    (_bounds.y - _bounds.x),
                    (verticalLocalPosition.x + QuadHorizontalWidth() / 2 - _bounds.x) /
                    (_bounds.y - _bounds.x)
                );
                break;
            case Mode.Vertical_Right:
                // verticalLocalPosition.x = verticalLocalPosition.x;
                p = new Vector2(
                    (verticalLocalPosition.x - QuadHorizontalWidth() / 2 - _bounds.x) /
                    (_bounds.y - _bounds.x),
                    (verticalLocalPosition.x + QuadHorizontalWidth() / 2 - _bounds.x) /
                    (_bounds.y - _bounds.x)
                );
                break;
        }

        return p;
    }

    public Vector2 GetNormalizedPosition()
    {
        Vector2 p = new Vector2(
            (transform.localPosition.x - _bounds.x) / (_bounds.y - _bounds.x),
            _normalizedHeight
        );
        return p;
    }
}